using DotNetty.Buffers;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
	[Packet(0x22, NetworkPlayerState.Play)]
	public class ChunkDataPacketOutPacket : IOutPacket
	{
		public int ByteSize;
		public int ChunkX;
		public int ChunkY;
		public byte[] Data;
		public bool FullChunk;

		public int NumberOfBlockEntities;

		public IByteBuffer overrideBuffer;

		public int PrimaryBitMask;
		//TODO: Block Entitys

		public void Write(IByteBuffer buffer)
		{
			//buffer.WriteInt(ChunkX);
			//buffer.WriteInt(ChunkY);
			//buffer.WriteBoolean(FullChunk);
			//buffer.WriteVarInt(PrimaryBitMask);
			//buffer.WriteVarInt(ByteSize);
			//buffer.WriteBytes(Data);
			//buffer.WriteVarInt(NumberOfBlockEntities);
			//TODO: Block Entities
			buffer.WriteBytes(overrideBuffer);
		}
	}
}