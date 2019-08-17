using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using Recube.Api.Entities;
using Recube.Api.Network.Impl.Packets.Play;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;
using Recube.Api.Util;
using Recube.Core.World;

namespace Recube.Core.Network.Impl
{
	public class PlayPacketHandler : PacketHandler
	{
		public static int entityID;
		private Player _player;

		public PlayPacketHandler(INetworkPlayer networkPlayer) : base(networkPlayer)
		{
		}

		public override void OnActive()
		{
			((NetworkPlayer.NetworkPlayer) NetworkPlayer).SetState(NetworkPlayerState.Play);
			entityID++;
			NetworkPlayer.SendPacketAsync(new JoinGameOutPacket
			{
				EntityId = entityID,
				Gamemode = 1,
				Dimension = 0,
				Difficulty = 1,
				MaxPlayers = 60,
				LevelType = "flat",
				ReducedDebugInfo = true
			});

			NetworkPlayer.SendPacketAsync(new SpawnPositionOutPacket
			{
				X = 1,
				Y = 170,
				Z = -4
			});
			for (var i = 0; i < 16; i++)
			{
				for (var j = 0; j < 16; j++)
				{
					var buf = ByteBufferUtil.DefaultAllocator.Buffer();
					var chunk = new Chunk();
					chunk.WriteChunkDataPacket(buf);
					_player.NetworkPlayer.SendPacketAsync(new ChunkDataPacketOutPacket
					{
						Data = buf.Array,
						ByteSize = buf.Array.Length,
						ChunkX = i,
						ChunkY = j,
						FullChunk = false,
						PrimaryBitMask = 0b0000_0000_0000_0000,
						NumberOfBlockEntities = 0
					});
				}
			}


			Console.WriteLine("OKKK");
			//_player.NetworkPlayer.FlushChannel();


			/*NetworkPlayer.SendPacketAsync(new PlayerPositionAndLookOutPacket
			{
				X = 0,
				Y = 100,
				Z = 0,
				Yaw = 0,
				Pitch = 0,
				Flags = 0,
				TeleportId = 1
			}); */
		}

		public override void OnDisconnect()
		{
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
	}
}