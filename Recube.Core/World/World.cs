using System;
using System.Collections.Generic;

namespace Recube.Core.World
{
    public class World : IDisposable
    {
        public string Name;
        public readonly List<Chunk> LoadedChunks;
        public WorldThread CurrentWorldThread { get; internal set; }

        public World(string name, List<Chunk> loadedChunks, WorldThread currentWorldThread)
        {
            Name = name;
            LoadedChunks = loadedChunks;
            CurrentWorldThread = currentWorldThread;
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