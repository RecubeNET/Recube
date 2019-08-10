using System.Threading.Tasks;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;
using Recube.Core.Network.Impl.Packets.Handshake;
using Recube.Core.Network.Impl.Packets.Status;
using Recube.Core.Network.Packets.Handler;

namespace Recube.Core.Network.Impl
{
	public class HandshakePacketHandler : PacketHandler
	{
		public HandshakePacketHandler(INetworkPlayer networkPlayer) : base(networkPlayer)
		{
		}

		public override void OnActive()
		{
			((NetworkPlayer.NetworkPlayer) NetworkPlayer).SetState(NetworkPlayerState.Handshake);
		}

		public override void OnDisconnect()
		{
		}

		public override Task Fallback(IInPacket packet)
		{
			return Task.CompletedTask;
		}

		[PacketMethod]
		public void OnHandshakingInPacket(HandshakeInPacket packet)
		{
			switch (packet.NextState)
			{
				case NetworkPlayerState.Status:
					NetworkPlayer.SetPacketHandler(
						PacketHandlerFactory.CreatePacketHandler(Recube.Instance.StatusPacketHandler, NetworkPlayer));
					break;
				case NetworkPlayerState.Login:
					NetworkPlayer.SetPacketHandler(
						PacketHandlerFactory.CreatePacketHandler(Recube.Instance.LoginPacketHandler, NetworkPlayer));
					break;
				default:
					NetworkPlayer.DisconnectAsync(); // OBVIOUSLY SOMETHING WENT WRONG
					break;
			}
		}

		[PacketMethod]
		public void OnPingInPacket(PingInPacket packet)
		{
			NetworkPlayer.SendPacketAsync(new PongOutPacket
			{
				Payload = packet.Payload
			});
		}
	}
}