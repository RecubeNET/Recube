using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using Recube.Api.Block.Impl;
using Recube.Api.Entities;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.Impl.Packets.Play;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;
using Recube.Api.Util;

namespace Recube.Core.Network.Impl
{
	public class PlayPacketHandler : PacketHandler
	{
		private Player _player;

		public PlayPacketHandler(INetworkPlayer networkPlayer) : base(networkPlayer)
		{
		}

		public override void OnActive()
		{
			((NetworkPlayer.NetworkPlayer) NetworkPlayer).SetState(NetworkPlayerState.Play);

			NetworkPlayer.SendPacketAsync(new JoinGameOutPacket
			{
				EntityId = 1,
				Gamemode = 1,
				Dimension = 0,
				Difficulty = 0,
				MaxPlayers = 0,
				LevelType = "",
				ReducedDebugInfo = false
			});

			for (int x = 0; x < 7; x++)
			{
				for (int y = 0; y < 7; y++)
				{
					var fullChunk = false;
					var bitMask = 0b0000_0000_0000_0001;
					var buf = ByteBufferUtil.DefaultAllocator.Buffer();
					for (int i = 0; i < 4095; i++)
					{
						var b = Recube.Instance.BlockStateRegistry
							.GetStateByProperty(typeof(GrassBlock), typeof(GrassBlock.SnowyProperty), false);
						buf.WriteVarInt(b.Id);
					}

					var blockEntities = 0;
					Task.Run(() =>
					{
						_player.NetworkPlayer.SendPacketAsync(new ChunkDataPacketOutPacket
						{
							ChunkX = x,
							ChunkY = y,
							FullChunk = false,
							PrimaryBitMask = bitMask,
							ByteSize = buf.ReadableBytes,
							Data = buf.Array,
							NumberOfBlockEntities = 0
						});
						buf.SafeRelease();
					});
				}
			}

			Console.WriteLine("OKKK");
			NetworkPlayer.SendPacketAsync(new SpawnPositionOutPacket
			{
				X = 0,
				Y = 100,
				Z = 0
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
		}

		public override void OnDisconnect()
		{
			if (_player == null) return;

			Recube.Instance.PlayerRegistry.Deregister(_player);
			_player.Remove();
			NetworkBootstrap.Logger.Info($"Player {_player.Username}[{_player.Uuid}] disconnected");
		}

		public override Task Fallback(IInPacket packet)
		{
			return Task.CompletedTask;
		}

		internal void SetPlayer(UUID uuid, string username)
		{
			_player = new Player(uuid, (NetworkPlayer.NetworkPlayer) NetworkPlayer, username);
			Recube.Instance.PlayerRegistry.Register(_player);
			NetworkBootstrap.Logger.Info(
				$"Player {_player.Username}[{_player.Uuid}] connected from {NetworkPlayer.Channel.RemoteAddress}");
		}
	}
}