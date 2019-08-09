using System;
using System.Threading.Tasks;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;

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
			throw new NotImplementedException();
		}

		public override Task Fallback(IInPacket packet)
		{
			throw new NotImplementedException();
		}
	}
}