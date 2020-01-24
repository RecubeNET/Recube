using DotNetty.Buffers;

namespace Recube.Api.World
{
    public interface IChunk
    {
        void Serialize(IByteBuffer buf);
    }
}