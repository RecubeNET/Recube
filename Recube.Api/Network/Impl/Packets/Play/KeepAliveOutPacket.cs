using DotNetty.Buffers;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
	[Packet(0x21, NetworkPlayerState.Play)]
	public class KeepAliveOutPacket : IOutPacket
	{
		public long Id;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteLong(Id);
		}
	}
}