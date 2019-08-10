using System;
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
		public string Username { get; set; }

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

		public Task SendPacketAsync(IOutPacket packet)
		{
			return Channel.WriteAndFlushAsync(packet);
		}

		public void SetState(NetworkPlayerState state)
		{
			CurrentState = state;
		}
	}
}