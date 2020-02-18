using System;
using DotNetty.Buffers;
using fNbt;
using Recube.Api;
using Recube.Api.Block;
using Recube.Api.Network.Extensions;
using Recube.Api.World;

namespace Recube.Core.World
{
    public class Chunk : IChunk
    {
        public readonly int X;
        public readonly int Z;

        private readonly ChunkSection[] _sections = new ChunkSection[16];
        private int _sectionMask;

        private readonly long[] _heightmap = new long[36];
        private readonly int[] _biomes = new int[1024];


        public Chunk(int x, int z)
        {
            X = x;
            Z = z;
            for (var i = 0; i < _sections.Length; i++)
                _sections[i] = ChunkSection.Build(new int [ChunkSection.DataArraySize]);

            for (var i = 0; i < _biomes.Length; i++) _biomes[i] = 127; // TODO ADD REAL BIOME SUPPORT

            for (var blockX = 0; blockX < 16; blockX++)
            for (var blockZ = 0; blockZ < 16; blockZ++)
                SetType(blockX, 0, blockZ, 9);
        }

        public void SetBlock(Location location, BaseBlock block)
        {
            var state = Recube.Instance.GetBlockStateRegistry().GetStateByBaseBlock(block);
            if (state == null)
            {
                Recube.RecubeLogger.Warn("Tried to place block with unknown BlockState");
                return;
            }

            SetType(location.X, location.Y, location.Z, state.NetworkId);
        }

        public BaseBlock? GetBlock(Location location)
        {
            var type = GetType(location.X, location.Y, location.Z);
            var state = Recube.Instance.GetBlockStateRegistry().GetBlockStateByNetworkId(type);
            if (state == null)
            {
                Recube.RecubeLogger.Warn($"Tried to get block whose state is unknown. Network id: {type}");
                return null;
            }

            return Recube.Instance.GetBlockStateRegistry().GetBaseBlockFromState(state);
        }

        public void SetType(int x, int y, int z, int type)
        {
            if (y > 256 || y < 0)
                throw new InvalidOperationException($"y ({y}) is bigger than 256 or less than 0"); // CHECK 

            var sectionIndex = (int) Math.Floor(y / 16d);
            var section = _sections[sectionIndex];
            section.SetType(x, y % 16, z, type);

            _sectionMask &= ~(1 << sectionIndex);
            if (section.BlockCount > 0)
                _sectionMask |= 1 << sectionIndex;
        }

        public int GetType(int x, int y, int z)
        {
            if (y > 256 || y < 0)
                throw new InvalidOperationException($"y ({y}) is bigger than 256 or less than 0"); // CHECK 

            var sectionIndex = (int) Math.Floor(y / 16d);
            if ((_sectionMask & (1 << sectionIndex)) == 0) return 0; // Section is completely empty => Block will be air

            var section = _sections[sectionIndex];
            return section.GetType(x, y, z);
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
    }
}