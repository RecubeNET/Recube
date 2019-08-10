using System;
using System.Threading.Tasks;
using Recube.Api.Network.Entities;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;
using Recube.Core.Network.Impl.Packets.Login;
using Recube.Core.Network.Impl.Packets.Start;

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
			throw new NotImplementedException();
		}

		[PacketMethod]
		public void OnLoginStartPacket(LoginStartPacket packet)
		{
			//TODO: Send Encryption Request
			//TODO: Dont Use Random UUID need something more unique cause username changes
			((NetworkPlayer.NetworkPlayer) NetworkPlayer).Username = packet.username;
			NetworkPlayer.SendPacketAsync(new LoginSuccessPacket()
			{
				Username = packet.username,
				UUID = new UUID()
			});
			((NetworkPlayer.NetworkPlayer) NetworkPlayer).SetState(NetworkPlayerState.Play);
		}
		[PacketMethod]
		public void OnEncryptionResponsePacket(EncryptionResponsePacket packet)
		{
			//TODO Enable Encryption
		}
	}
}