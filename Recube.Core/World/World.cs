using System;
using System.Collections.Generic;
using System.Threading;
using Recube.Api.World;

namespace Recube.Core.World
{
    public class World: IDisposable
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

        internal void Tick() {}

        public void SaveToDisk() {}

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            SaveToDisk();
        }
    }
}