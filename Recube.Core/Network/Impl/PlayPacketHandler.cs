using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Timers;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using Recube.Api.Entities;
using Recube.Api.Network.Impl.Packets.Play;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;
using Recube.Api.Util;
using Recube.Api.World;

namespace Recube.Core.Network.Impl
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class PlayPacketHandler : PacketHandler
	{
		private static readonly Random _random = new Random();
		private long? _keepAliveId;
		private DateTime? _lastPong;
		private Player _player;
		private Timer? _timeoutTimer;

		public PlayPacketHandler(INetworkPlayer networkPlayer) : base(networkPlayer)
		{
		}

		public override void OnActive()
		{
			_lastPong = DateTime.Now;
			_timeoutTimer = new Timer(10000) {AutoReset = true};
			_timeoutTimer.Elapsed += async (sender, args) =>
			{
				var now = DateTime.Now;
				if (_lastPong == null)
				{
					await _player.NetworkPlayer.DisconnectAsync();
					return;
				}

				var diff = now - _lastPong.Value;
				if (diff.TotalSeconds >= 30)
				{
					await _player.NetworkPlayer.DisconnectAsync();
					return;
				}

				_keepAliveId = _random.NextLong();
				await NetworkPlayer.SendPacketAsync(new KeepAliveOutPacket {Id = (long) _keepAliveId});
			};
			_timeoutTimer.Start();

			((NetworkPlayer.NetworkPlayer) NetworkPlayer).SetState(NetworkPlayerState.Play);
			NetworkPlayer.SendPacketAsync(new JoinGameOutPacket
			{
				EntityId = _player.EntityId,
				ViewDistance = 2,
				Gamemode = 1,
				Dimension = 0,
				MaxPlayers = 60,
				LevelType = "flat",
				ReducedDebugInfo = false,
				HashedSeed = 2483274872339L,
				EnableRespawnScreen = true
			});

			NetworkPlayer.SendPacketAsync(new SpawnPositionOutPacket
			{
				BlockPosition = new BlockPosition(1, 170, -4)
			});

			NetworkPlayer.SendPacketAsync(new PlayerPositionAndLookOutPacket
			{
				X = 0,
				Y = 100,
				Z = 0,
				Yaw = 0,
				Pitch = 0,
				Flags = 0,
				TeleportId = 1
			});

			//TODO: This needs to be fixed!
			for (var i = -12; i < 12; i++)
			{
				for (var j = -12; j < 12; j++)
				{
					var buffer = ByteBufferUtil.DefaultAllocator.Buffer();
					var chunk = new Chunk(i, j);
					chunk.WriteChunkDataPacket(buffer);
					_player.NetworkPlayer.SendPacketAsync(new ChunkDataPacketOutPacket
					{
						Chunk = chunk
					});
				}
			}

			Console.WriteLine("OKKK");
			//_player.NetworkPlayer.FlushChannel();
		}

		public override void OnDisconnect()
		{
			_timeoutTimer?.Stop();

			if (_player == null) return;

			Recube.Instance.PlayerRegistry.Deregister(_player);
			Recube.Instance.EntityRegistry.DeregisterEntity(_player.EntityId);
			NetworkBootstrap.Logger.Info($"Player {_player.Username}[{_player.Uuid}] disconnected");
		}

		public override Task Fallback(IInPacket packet)
		{
			return Task.CompletedTask;
		}

		internal void SetPlayer(Uuid uuid, string username)
		{
			_player = (Player) Recube.Instance.EntityRegistry.RegisterEntity(id =>
				new Player(id, uuid, NetworkPlayer, username));
			Recube.Instance.PlayerRegistry.Register(_player);
			NetworkBootstrap.Logger.Info(
				$"Player {_player.Username}[{_player.Uuid}] connected from {NetworkPlayer.Channel.RemoteAddress}");
		}

		[PacketMethod]
		public void OnKeepAliveInOutPacket(KeepAliveInPacket packet)
		{
			var neededId = _keepAliveId ?? 0;
			if (neededId != packet.Id) return;
			_lastPong = DateTime.Now;
		}
	}
}