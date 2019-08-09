using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Impl.Packets.Status
{
	[Packet(0x0, NetworkPlayerState.Status)]
	public class ResponseOutPacket : IOutPacket
	{
		public string JsonResponse;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteStringWithLength(JsonResponse);
		}
	}
}