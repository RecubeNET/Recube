using System;
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
			if (!Channel.Active) return;

			PacketHandler.OnDisconnect();
			await Channel.CloseAsync().ConfigureAwait(false);
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
			if (!Channel.Active) return;

			try
			{
				await Channel.WriteAndFlushAsync(packet);
			}
			catch (ChannelClosedException ignored)
			{
			}
		}

		public async Task WriteAsync(IOutPacket packet)
		{
			if (!Channel.Active) return;
			try
			{
				await Channel.WriteAsync(packet);
			}
			catch (ChannelClosedException ignored)
			{
			}
		}

		public void FlushChannel()
		{
			if (!Channel.Active) return;
			try
			{
				Channel.Flush();
			}
			catch (ChannelClosedException ignored)
			{
			}
		}

		public void SetState(NetworkPlayerState state)
		{
			CurrentState = state;
		}
	}
}