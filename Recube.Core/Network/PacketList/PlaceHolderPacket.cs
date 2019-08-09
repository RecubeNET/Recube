using System;
using DotNetty.Buffers;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.PacketList
{
	[Packet(0, NetworkPlayerState.Handshake)]
	public class PlaceHolderPacket : IInOutPacket
	{
		public void Read(IByteBuffer buffer)
		{
			throw new NotImplementedException();
		}

		public void Write(IByteBuffer buffer)
		{
			throw new NotImplementedException();
		}
	}
}