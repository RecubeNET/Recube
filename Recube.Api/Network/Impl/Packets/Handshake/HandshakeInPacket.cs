using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Handshake
{
	// https://wiki.vg/Protocol#Handshake
	[Packet(0x0, NetworkPlayerState.Handshake)]
	public class HandshakeInPacket : IInPacket
	{
		public NetworkPlayerState NextState;
		public int ProtocolVersion;

		public void Read(IByteBuffer buffer)
		{
			ProtocolVersion = buffer.ReadVarInt();
			buffer.ReadStringWithLength(); // DISCARD SERVER ADDRESS
			buffer.ReadUnsignedShort(); // DISCARD SERVER PORT
			NextState = (NetworkPlayerState) buffer.ReadVarInt();
		}
	}
}