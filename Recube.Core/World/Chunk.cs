using System;
using System.IO;
using DotNetty.Buffers;
using fNbt;
using Recube.Api.Network.Extensions;
using Recube.Api.Util;
using Recube.Api.World;

namespace Recube.Core.World
{
    public class Chunk : IChunk
    {
        public int X;
        public int Z;

        public Chunk(int x, int z)
        {
            X = x;
            Z = z;
        }

        public void Serialize(IByteBuffer data)
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

            var compound = new NbtCompound("");
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
            var secLongs = new long[14 * 64];
            uint mask = (1 << 14) - 1;
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        var blockNum = (y * 16 + z) * 16 + x;
                        var startLong = blockNum * 14 / 64;
                        var startOffset = blockNum * 14 % 64;
                        var endLong = ((blockNum + 1) * 14 - 1) / 64;

                        long value = 9 & mask;
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
    }
}