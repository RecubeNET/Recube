using System;
using DotNetty.Buffers;
using fNbt;
using Recube.Api.Network.Extensions;
using Recube.Api.World;

namespace Recube.Core.World
{
    public class Chunk : IChunk
    {
        private readonly long[] _heightmap = new long[36];
        private readonly int _sectionMask = 1;

        private readonly ChunkSection[] _sections = new ChunkSection[16];
        public int X;
        public int Z;

        public Chunk(int x, int z)
        {
            X = x;
            Z = z;
            for (var i = 0; i < _sections.Length; i++)
            {
                _sections[i] = ChunkSection.Build(new int [ChunkSection.DataArraySize]);
                _sections[i].SetType(0, 0, 0, 9);
                _sections[i].SetType(15, 0, 0, 9);
                /* _sections[i].SetType(0, 0, 16, 9);
                _sections[i].SetType(16, 0, 16, 9);*/
            }
        }

        public void Serialize(IByteBuffer data)
        {
            data.WriteInt(X);
            data.WriteInt(Z);
            data.WriteBoolean(true);
            data.WriteVarInt(_sectionMask);

            var compound = new NbtCompound("") {new NbtLongArray("MOTION_BLOCKING", _heightmap)};
            data.WriteBytes(new NbtFile(compound).SaveToBuffer(NbtCompression.None));

            var biomes = new int[1024];
            for (var I = 0; I < biomes.Length; I++) biomes[I] = 127;

            data.WriteIntArray(biomes);

            var secBuf = Unpooled.Buffer();
            for (var i = 0; i < _sections.Length; i++)
            {
                if ((_sectionMask & (1 << i)) == 0) continue;
                Console.WriteLine($"{X} : {Z}");
                _sections[i].Serialize(secBuf);
            }

            data.WriteVarInt(secBuf.ReadableBytes);
            data.WriteBytes(secBuf);

            data.WriteVarInt(0);
        }
    }
}