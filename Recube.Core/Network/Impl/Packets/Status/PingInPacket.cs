using DotNetty.Buffers;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Impl.Packets.Status
{
	[Packet(0x1, NetworkPlayerState.Status)]
	public class PingInPacket : IInPacket
	{
		public long Payload;

		public void Read(IByteBuffer buffer)
		{
			Payload = buffer.ReadLong();
		}
	}
}