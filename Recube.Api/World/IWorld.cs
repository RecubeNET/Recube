using System;
using System.Threading.Tasks;
using Recube.Api.Block;

namespace Recube.Api.World
{
    public interface IWorld
    {
        public string Name { get; }
        public void SetBlock(Location location, BaseBlock block);

        public BaseBlock GetBlock(Location location);

        public T? GetBlock<T>(Location location) where T : BaseBlock;

        public void SetType(int x, int y, int z, int type);
        public int GetType(int x, int y, int z);


        public Task Run(Func<Task> action);
    }
}