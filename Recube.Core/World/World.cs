using System;
using System.Threading;
using Recube.Api.World;

namespace Recube.Core.World
{
    public class World: IDisposable
    {
        public string Name;
        public WorldThread CurrentWorldThread { get; internal set; }

        internal void Tick() {}
        
        public void SetBlock()
        {
        }

        public void SaveToDisk() {}

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            SaveToDisk();
        }
    }
}