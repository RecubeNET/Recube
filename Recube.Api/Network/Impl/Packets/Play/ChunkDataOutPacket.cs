using DotNetty.Buffers;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.World;

namespace Recube.Api.Network.Impl.Packets.Play
{
	[Packet(0x22, NetworkPlayerState.Play)]
	public class ChunkDataPacketOutPacket : IOutPacket
	{
		public Chunk Chunk;

		public void Write(IByteBuffer buffer)
		{
			Chunk.WriteChunkDataPacket(buffer);
		}
	}
}