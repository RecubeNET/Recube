using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Login
{
	[Packet(0x3, NetworkPlayerState.Login)]
	public class SetCompressionPacket : IOutPacket
	{
		public int Threshold;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteVarInt(Threshold);
		}
	}
}