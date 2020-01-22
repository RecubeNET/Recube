using System;
using System.IO;
using DotNetty.Buffers;
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
			data.WriteBytes(File.ReadAllBytes("chunkpacket.dat"));
			/*data.WriteBoolean(false);
			data.WriteVarInt((1 << 15));

			data.WriteBytes(File.ReadAllBytes("testheightmap.dat"));

			var chunkData = ByteBufferUtil.DefaultAllocator.Buffer();

			chunkData.WriteShort(4096);
			chunkData.WriteByte(14);

			var dat = new long[(4096 * 14) / 64];
			for (int i = 0; i < dat.Length; i++)
			{
				dat[i] = 3328328232;
			}
			/*var biomes = new int[1024];
			for (int i = 0; i < biomes.Length; i++)
			{
				biomes[i] = 127;
			}#1#

			chunkData.WriteVarInt(dat.Length);
			chunkData.WriteLongArray(dat);
			
			/*data.WriteIntArray(biomes);#1#
			data.WriteVarInt(chunkData.WritableBytes);
			data.WriteBytes(chunkData);
			data.WriteVarInt(0);*/
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