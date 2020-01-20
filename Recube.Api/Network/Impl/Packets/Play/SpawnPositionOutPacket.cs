using DotNetty.Buffers;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Util;

namespace Recube.Api.Network.Impl.Packets.Play
{
	[Packet(0x4E, NetworkPlayerState.Play)]
	public class SpawnPositionOutPacket : IOutPacket
	{
		public BlockPosition BlockPosition;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteLong(BlockPosition.ToLong());
		}
	}
}