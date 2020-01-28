using System;
using DotNetty.Buffers;
using fNbt;
using Recube.Api.Network.Extensions;
using Recube.Api.World;

namespace Recube.Core.World
{
    public class Chunk : IChunk
    {
        private readonly int[] _biomes = new int[1024];

        private readonly long[] _heightmap = new long[36];
        private readonly ChunkSection[] _sections = new ChunkSection[16];
        public readonly int X;
        public readonly int Z;

        private int _sectionMask;

        public Chunk(int x, int z)
        {
            X = x;
            Z = z;
            for (var i = 0; i < _sections.Length; i++)
                _sections[i] = ChunkSection.Build(new int [ChunkSection.DataArraySize]);

            for (var i = 0; i < _biomes.Length; i++) _biomes[i] = 127; // TODO ADD REAL BIOME SUPPORT

            for (var blockX = 0; blockX < 16; blockX++)
            for (var blockZ = 0; blockZ < 16; blockZ++)
                SetBlock(blockX, 0, blockZ, 9);
        }

        public void Serialize(IByteBuffer data)
        {
            data.WriteInt(X);
            data.WriteInt(Z);
            data.WriteBoolean(true);
            data.WriteVarInt(_sectionMask);

            var compound = new NbtCompound("") {new NbtLongArray("MOTION_BLOCKING", _heightmap)};
            data.WriteBytes(new NbtFile(compound).SaveToBuffer(NbtCompression.None));

            data.WriteIntArray(_biomes);

            var secBuf = Unpooled.Buffer();
            for (var i = 0; i < _sections.Length; i++)
            {
                if ((_sectionMask & (1 << i)) == 0) continue;
                _sections[i].Serialize(secBuf);
            }

            data.WriteVarInt(secBuf.ReadableBytes);
            data.WriteBytes(secBuf);

            data.WriteVarInt(0);
        }

        public void SetBlock(int x, int y, int z, int type)
        {
            if (y > 256 || y < 0)
                throw new InvalidOperationException($"y ({y}) is bigger than 256 or less than 0"); // CHECK 

            var sectionIndex = (int) Math.Floor(y / 16d);
            var section = _sections[sectionIndex];
            section.SetType(x % 16, y % 16, z % 16, type);

            _sectionMask &= ~(1 << sectionIndex);
            if (section.BlockCount > 0)
                _sectionMask |= 1 << sectionIndex;
        }

        public int GetBlock(int x, int y, int z)
        {
            if (y > 256 || y < 0)
                throw new InvalidOperationException($"y ({y}) is bigger than 256 or less than 0"); // CHECK 

            var sectionIndex = (int) Math.Floor(y / 16d);
            if ((_sectionMask & (1 << sectionIndex)) == 0) return 0; // Section is completely empty => Block will be air

            var section = _sections[sectionIndex];
            return section.GetType(x, y, z);
        }
    }
}