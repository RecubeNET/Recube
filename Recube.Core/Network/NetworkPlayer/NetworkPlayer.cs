using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;

namespace Recube.Core.Network.NetworkPlayer
{
	public class NetworkPlayer : INetworkPlayer
	{
		private SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

		public NetworkPlayer(IChannel channel)
		{
			Channel = channel;
			CurrentState = NetworkPlayerState.Handshake;
		}

		public IChannel Channel { get; }
		public PacketHandler PacketHandler { get; private set; }
		public NetworkPlayerState CurrentState { get; private set; }

		public async Task DisconnectAsync()
		{
			await _lock.WaitAsync();
			try
			{
				if (!Channel.Active) return;

				PacketHandler.OnDisconnect();
				await Channel.CloseAsync().ConfigureAwait(false);
			}
			finally
			{
				_lock.Release();
			}
		}

		public void SetPacketHandler(PacketHandler packetHandler)
		{
			PacketHandler =
				packetHandler ??
				throw new ArgumentNullException(nameof(packetHandler), "PacketHandler must not be null");
			PacketHandler.OnActive();
		}

		public async Task SendPacketAsync(IOutPacket packet)
		{
			await _lock.WaitAsync();
			try
			{
				NetworkBootstrap.Logger.Debug($"Sent packet {packet.GetType().FullName}");
				if (!Channel.Active) return;

				try
				{
					await Channel.WriteAndFlushAsync(packet);
				}
				catch (ChannelClosedException)
				{
				}
			}
			finally
			{
				_lock.Release();
			}
		}

		public void SetState(NetworkPlayerState state)
		{
			_lock.Wait();
			try
			{
				CurrentState = state;
			}
			finally
			{
				_lock.Release();
			}
		}
	}
}