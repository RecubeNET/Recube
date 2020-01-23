using System;
using System.IO;
using DotNetty.Buffers;
using fNbt;
using Recube.Api.Network.Extensions;
using Recube.Api.Util;

namespace Recube.Api.World
{
	public class Chunk : IDisposable
	{
		//TODO: Rewrite this class
		private const int ChunkSize = 256;
		private const int SectionSize = 16;
		public byte[] BiomeId = ArrayOf<byte>.Create(256, 1);

		public long InhabitedTime = 0;
		public long LastUpdate = 0;
		public bool LightPopulated = false;

		public ChunkSection[] Sections = new ChunkSection[16]
		{
			new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(),
			new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(),
			new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(), new ChunkSection(),
			new ChunkSection()
		};

		public bool TerrainPopulated = false;

		public int X;
		public int Z;

		public Chunk(int x, int z)
		{
			X = x;
			Z = z;
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void WriteChunkDataPacket(IByteBuffer data)
		{
			data.WriteInt(X);
			data.WriteInt(Z);
			//	data.WriteBytes(File.ReadAllBytes("chunkpacket.dat"));
			data.WriteBoolean(true);
			data.WriteVarInt(1 << 0);
			var heightmap = new long[36];
			for (var I = 0; I < heightmap.Length; I++)
			{
				heightmap[I] = 16;
			}

			var compound = new NbtCompound("hello world");
			compound.Add(new NbtLongArray("MOTION_BLOCKING", heightmap));
			data.WriteBytes(new NbtFile(compound).SaveToBuffer(NbtCompression.None));

			var biomes = new int[1024];
			for (var I = 0; I < biomes.Length; I++)
			{
				biomes[I] = 127;
			}
			data.WriteIntArray(biomes);
			
			// DATA
			var secBuf = Unpooled.Buffer();
			var secLongs = new long[16 * 16 * 16 * 14 / 64];
			uint mask = (1 << 14) - 1;
			for (int x = 0; x < 16; x++)
			{
				for (int z = 0; z < 16; z++)
				{
					for (int y = 0; y < 16; y++)
					{
						var blockNum = (x * 16 + z) * 16 + y;
						var startLong = blockNum * 14 / 64;
						var startOffset = blockNum * 14 % 64;
						var endLong = ((blockNum + 1) * 14 - 1) / 64;

						var value = 9 & mask;
						secLongs[startLong] |= value << startOffset;

						if (startLong != endLong)
						{
							secLongs[endLong] = value >> (64 - startOffset);
						}
					}
				}
			}

			secBuf.WriteShort(4096);
			secBuf.WriteByte(14);
			secBuf.WriteVarInt(secLongs.Length);
			secBuf.WriteLongArray(secLongs);
			data.WriteVarInt(secBuf.ReadableBytes);
			data.WriteBytes(secBuf);

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
		}

		public byte GetBlock(in int x, in int y, in int z)
		{
			return 0x0;
		}

		public byte GetMetadata(in int x, in int y, in int z)
		{
			return 0x0;
		}

		public byte GetBlockLight(in int x, in int y, in int z)
		{
			return byte.MaxValue;
		}

		public byte GetSkylight(in int x, in int y, in int z)
		{
			return byte.MaxValue;
		}
	}
}