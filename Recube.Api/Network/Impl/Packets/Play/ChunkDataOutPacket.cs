using DotNetty.Buffers;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Core.World;

namespace Recube.Api.Network.Impl.Packets.Play
{
	[Packet(0x22, NetworkPlayerState.Play)]
	public class ChunkDataPacketOutPacket : IOutPacket
	{
		private Chunk chunk;

		public int PrimaryBitMask;
		//TODO: Block Entitys

		public void Write(IByteBuffer buffer)
		{
			chunk.WriteChunkDataPacket(buffer);
		}
	}
}