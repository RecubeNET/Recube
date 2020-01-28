using System;
using DotNetty.Buffers;
using Recube.Core.World;
using Xunit;

namespace Recube.Core.Tests.World
{
    public class ChunkSectionTest
    {
        // Creating Tests
        [Fact]
        public void Create()
        {
            Assert.Throws<InvalidOperationException>(() => ChunkSection.Build(new int[0]));
            Assert.True(ChunkSection.ChunkHeight >= 1);
            Assert.True(ChunkSection.ChunkWidth >= 1);
            var array = new int[ChunkSection.ChunkWidth * ChunkSection.ChunkWidth * ChunkSection.ChunkHeight];
            Assert.True(ChunkSection.DataArraySize == array.Length);
            array[0] = 1;
            array[1] = Int32.MaxValue;
            array[2] = Int32.MinValue;
            var chunk1 = ChunkSection.Build(array);
            chunk1.Recount();
            chunk1.Optimize();
            for (int i = 3; i < 4096; i++)
            {
                array[i] = i;
            }

            Assert.Throws<OverflowException>(() => ChunkSection.Build(array));
            chunk1.Recount();
            chunk1.Optimize();

            chunk1.SetType(0, 0, 0, 0);
            Assert.Throws<InvalidOperationException>(() => chunk1.SetType(ChunkSection.ChunkWidth + 1, 0, 0, 0));
            Assert.Throws<InvalidOperationException>(() => chunk1.SetType(0, ChunkSection.ChunkHeight + 1, 0, 0));
            Assert.Throws<InvalidOperationException>(() => chunk1.SetType(0, 0, ChunkSection.ChunkWidth + 1, 0));
            chunk1.SetType(0, 0, 0, 1);
            chunk1.SetType(0, 0, 0, 6000);
            array = new int[ChunkSection.ChunkWidth * ChunkSection.ChunkWidth * ChunkSection.ChunkHeight];
            chunk1 = ChunkSection.Build(array);
            var buffer = Unpooled.Buffer();
            chunk1.Serialize(buffer);
            Assert.True(buffer.ReadableBytes == 2055);
            
            //TODO: THis Crashes
            /*for (int i = 0; i < 4096; i++)
            {
                chunk1.SetType(0,0,0, i);
            }*/

        }
    }
}