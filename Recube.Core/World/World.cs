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

        public bool SetBlock(Location location, BaseBlock block)
        {
            var state = block.AsBlockState();
            if (state == null) return false;

            SetType(location.X, location.Y, location.Z, state.NetworkId);
            block.Name = state.BaseName;
            block.World = this;
            block.Location = location;

            return true;
        }

        public BaseBlock? GetBlock(Location location)
        {
            var state = Recube.Instance.GetBlockStateRegistry()
                .GetBlockStateByNetworkId(GetType(location.X, location.Y, location.Z));
            return state == null ? null : Recube.Instance.GetBlockStateRegistry().GetBaseBlockFromState(state);
        }

        public T? GetBlock<T>(Location location) where T : BaseBlock
        {
            var block = GetBlock(location);
            if (block == null) return null;
            return (T) (block is T ? block : null);
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