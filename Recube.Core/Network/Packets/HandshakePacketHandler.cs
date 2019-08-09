using System;
using System.Threading.Tasks;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;

namespace Recube.Core.Network.Packets
{
	public class HandshakePacketHandler : PacketHandler
	{
		public HandshakePacketHandler(INetworkPlayer networkPlayer) : base(networkPlayer)
		{
		}

		public override void OnActive()
		{
			throw new NotImplementedException();
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