using DotNetty.Buffers;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Impl.Packets.Status
{
	[Packet(0x0, NetworkPlayerState.Status)]
	public class RequestInPacket : IInPacket
	{
		public void Read(IByteBuffer buffer)
		{
		}
	}
}