using DotNetty.Buffers;
using Recube.Api.Network.Extensions;

namespace Recube.Core.World
{
	public class Chunk
	{
		private readonly int CHUNK_SIZE = 256;
		private readonly int SECTION_SIZE = 16;

		public int ChunkX;
		public int ChunkZ;

		public ChunkSection[] Sections = new ChunkSection[16]
		{
			new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(),
			new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(),
			new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(),
			new ChunkSection()
		};

		public void WriteChunkDataPacket(IByteBuffer data)
		{
			data.WriteInt(ChunkX);
			data.WriteInt(ChunkZ);
			data.WriteBoolean(true); // FULL CHUNK
			var mask = 0;
			var columnBuffer = ByteBufferUtil.DefaultAllocator.Buffer();
			//TODO: THis

			for (var sectionY = 0; sectionY < CHUNK_SIZE / SECTION_SIZE; sectionY++)
			{
				if (!IsSectionEmpty(sectionY))
				{
					mask |= 1 << sectionY; // Set that bit to true in the mask
					WriteChunkSection(Sections[sectionY], columnBuffer);
				}
			}

			for (var z = 0; z < SECTION_SIZE; z++)
			{
				for (var x = 0; x < SECTION_SIZE; x++)
				{
					columnBuffer.WriteInt(GetBiome(x, z)); // Use 127 for 'void' if your server doesn't support biomes
				}
			}

			data.WriteVarInt(mask);
			data.WriteVarInt(columnBuffer.Capacity);
			data.WriteBytes(columnBuffer);

			// If you don't support block entities yet, use 0
			// If you need to implement it by sending block entities later with the update block entity packet,
			// do it that way and send 0 as well.  (Note that 1.10.1 (not 1.10 or 1.10.2) will not accept that)

			//TODO: Send BlockEntitties
			/*data.WriteVarInt(chunk.BlockEntities.Length););
			foreach (CompoundTag tag in chunk.BlockEntities)
			{
				WriteCompoundTag(data, tag);
			}*/
			data.WriteVarInt(0);
		}

		private int GetBiome(in int x, in int z)
		{
			//TODO Return proper Biome
			return 127;
		}

		private bool IsSectionEmpty(in int sectionY)
		{
			//TODO: Return proper response
			return true;
		}


		private void WriteChunkSection(ChunkSection section, IByteBuffer buf)
		{
			var palette = section.Palette;
			var bitsPerBlock = palette.GetBitsPerBlock();

			buf.WriteByte(bitsPerBlock);
			palette.Write(buf);

			// See tips section for an explanation of this calculation
			var dataLength = 16 * 16 * 16 * bitsPerBlock / 64;
			var data = new ulong[dataLength];

			// A bitmask that contains bitsPerBlock set bits
			var individualValueMask = (uint) ((1 << bitsPerBlock) - 1);

			for (var y = 0; y < SECTION_SIZE; y++)
			{
				for (var z = 0; z < SECTION_SIZE; z++)
				{
					for (var x = 0; x < SECTION_SIZE; x++)
					{
						var blockNumber = (y * SECTION_SIZE + z) * SECTION_SIZE + x;
						var startLong = blockNumber * bitsPerBlock / 64;
						var startOffset = blockNumber * bitsPerBlock % 64;
						var endLong = ((blockNumber + 1) * bitsPerBlock - 1) / 64;
						//TODO: BlockState
						var block = section.GetBaseBlock(x, y, z);

						ulong value = palette.IdForState(Recube.Instance.BlockStateRegistry.GetStateByBaseBlock(block));
						value &= individualValueMask;

						data[startLong] |= value << startOffset;

						if (startLong != endLong)
						{
							data[endLong] = value >> (64 - startOffset);
						}
					}
				}
			}

			buf.WriteVarInt(dataLength);
			buf.WriteULongArray(data);

			for (var y = 0; y < SECTION_SIZE; y++)
			{
				for (var z = 0; z < SECTION_SIZE; z++)
				{
					for (var x = 0; x < SECTION_SIZE; x += 2)
					{
						// Note: x += 2 above; we read 2 values along x each time
						var ligntValue =
							(byte) (section.GetBlockLight(x, y, z) | (section.GetBlockLight(x + 1, y, z) << 4));
						buf.WriteByte(ligntValue);
					}
				}
			}

			//TODO: Check for current Dimention
			if (false)
			{
				// IE, current dimension is overworld / 0
				for (var y = 0; y < SECTION_SIZE; y++)
				{
					for (var z = 0; z < SECTION_SIZE; z++)
					{
						for (var x = 0; x < SECTION_SIZE; x += 2)
						{
							// Note: x += 2 above; we read 2 values along x each time
							var skyLight =
								(byte) (section.GetSkyLight(x, y, z) | (section.GetSkyLight(x + 1, y, z) << 4));
							buf.WriteByte(skyLight);
						}
					}
				}
			}
		}
	}
}