using System.Threading.Tasks;
using Recube.Api.Network.Impl.Packets.Login;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;
using Recube.Api.Util;
using Recube.Core.Network.Packets.Handler;

namespace Recube.Core.Network.Impl
{
	public class LoginPacketHandler : PacketHandler
	{
		public LoginPacketHandler(INetworkPlayer networkPlayer) : base(networkPlayer)
		{
		}

		public override void OnActive()
		{
			((NetworkPlayer.NetworkPlayer) NetworkPlayer).SetState(NetworkPlayerState.Login);
		}

		public override void OnDisconnect()
		{
			// TODO: On Disconnect
		}

		public override Task Fallback(IInPacket packet)
		{
			return Task.CompletedTask;
		}

		[PacketMethod]
		public void OnLoginStartPacket(LoginStartPacket packet)
		{
			//TODO: Add Disconnect packet and event to disconnect
			//TODO: Send Encryption Request
			//TODO: Dont Use Random UUID need something more unique cause username changes
			var uuid = new UUID();

			Task.Run(async () =>
			{
				await NetworkPlayer.SendPacketAsync(new LoginSuccessPacket
				{
					Username = packet.Username,
					Uuid = uuid
				});

				NetworkPlayer.SetPacketHandler(
					PacketHandlerFactory.CreatePacketHandler(Recube.Instance.PlayPacketHandler, NetworkPlayer));
				((PlayPacketHandler) NetworkPlayer.PacketHandler).SetPlayer(uuid, packet.Username);
			});
		}

		[PacketMethod]
		public void OnEncryptionResponsePacket(EncryptionResponsePacket packet)
		{
			//TODO Enable Encryption
		}
	}
}