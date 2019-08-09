using DotNetty.Buffers;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Impl.Packets.Status
{
	[Packet(0x1, NetworkPlayerState.Status)]
	public class PongOutPacket : IOutPacket
	{
		public long Payload;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteLong(Payload);
		}
	}
}