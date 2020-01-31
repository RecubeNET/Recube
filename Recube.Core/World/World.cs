using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Recube.Api;
using Recube.Api.Block;
using Recube.Api.World;

namespace Recube.Core.World
{
    public class World : IWorld, IDisposable
    {
        public string Name { get; private set; }
        public readonly List<Chunk> LoadedChunks;
        public WorldThread CurrentWorldThread { get; internal set; }

        public World(string name, List<Chunk> loadedChunks)
        {
            Name = name;
            LoadedChunks = loadedChunks;
        }

        public void SetType(int x, int y, int z, int type)
        {
            var chunkX = (int) Math.Floor(x / 16d);
            var chunkZ = (int) Math.Floor(z / 16d);
            var chunk = LoadedChunks.Find(loadedChunk => loadedChunk.X == chunkX && loadedChunk.Z == chunkZ);

            chunk?.SetType(x % 16, y, z % 16, type);
        }

        public int GetType(int x, int y, int z)
        {
            var chunkX = (int) Math.Floor(x / 16d);
            var chunkZ = (int) Math.Floor(z / 16d);

            return LoadedChunks
                       .Find(loadedChunk => loadedChunk.X == chunkX && loadedChunk.Z == chunkZ)
                       ?.GetType(x % 16, y, z % 16) ?? 0;
        }

        public void SetBlock(Location location, BaseBlock block)
        {
            throw new NotImplementedException();
        }

        public BaseBlock GetBlock(Location location)
        {
            throw new NotImplementedException();
        }

        public T? GetBlock<T>(Location location) where T : BaseBlock
        {
            throw new NotImplementedException();
        }

        public Task Run(Func<Task> action) => CurrentWorldThread.Execute(action);


        public Chunk? GetChunk(int chunkX, int chunkZ)
            => LoadedChunks.Find(loadedChunk => loadedChunk.X == chunkX && loadedChunk.Z == chunkZ);


        public Chunk? GetChunkByGlobalCoords(int x, int z) =>
            GetChunk((int) Math.Floor(x / 16d), (int) Math.Floor(z / 16d));


        internal void Tick()
        {
        }

        public void SaveToDisk()
        {
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            SaveToDisk();
        }
    }
}