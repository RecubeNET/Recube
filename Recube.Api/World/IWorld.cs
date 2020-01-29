using System;
using System.Threading.Tasks;

namespace Recube.Api.World
{
    public interface IWorld
    {
        public string Name { get; }
        public void SetType(int x, int y, int z, int type);
        public int GetType(int x, int y, int z);

        public Task Run(Func<Task> action);
    }
}