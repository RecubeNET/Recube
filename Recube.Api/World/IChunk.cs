using DotNetty.Buffers;
using Recube.Api.Block;

namespace Recube.Api.World
{
    public interface IChunk
    {
        void SetBlock(Location location, BaseBlock block);

        BaseBlock? GetBlock(Location location);

        void SetType(int x, int y, int z, int type);

        int GetType(int x, int y, int z);

        void Serialize(IByteBuffer buf);
    }
}