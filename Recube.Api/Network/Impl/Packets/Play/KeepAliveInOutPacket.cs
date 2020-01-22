using DotNetty.Buffers;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
	[Packet(0x0F, NetworkPlayerState.Play)]
	public class KeepAliveInPacket: IInPacket
	{
		public long Id;
		
		public void Read(IByteBuffer buffer)
		{
			Id = buffer.ReadLong();
		}
	}
}