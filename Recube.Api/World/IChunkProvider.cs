using System.Threading.Tasks;

namespace Recube.Api.World
{
    public interface IChunkProvider
    {
        public IChunk GetChunk(int chunkX, int chunkZ);
    }
}