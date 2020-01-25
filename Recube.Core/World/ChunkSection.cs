using System.Collections.Generic;
using DotNetty.Buffers;

namespace Recube.Core.World
{
    public class ChunkSection
    {
        public const int GlobalPaletteBitsPerBlock = 14;
        public const int ChunkWidth = 16;
        public const int ChunkHeight = 16;
        public const int DataArraySize = ChunkWidth * ChunkWidth * ChunkHeight;
        
        private VariableBlockArray _data;
        private LinkedList<int>? _palette;

        public void Serialize(IByteBuffer buf)
        {
        
        }
    }
}