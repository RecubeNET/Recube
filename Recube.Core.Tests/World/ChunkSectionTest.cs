using System;
using System.Linq;
using Recube.Core.World;
using Xunit;

namespace Recube.Core.Tests.World
{
    public class ChunkSectionTest
    {
        [Fact]
        public void BlockCountTest()
        {
            var sec = ChunkSection.Build(new int[4096]);
            Assert.Equal(0, sec.BlockCount);

            var arr = new int[4096];
            for (var i = 0; i < arr.Length; i++) arr[i] = 1;

            var sec2 = ChunkSection.Build(arr);
            Assert.Equal(4096, sec2.BlockCount);

            sec2.SetType(0, 0, 0, 0);
            Assert.Equal(4095, sec2.BlockCount);

            sec2.SetType(0, 0, 0, 11000);
            Assert.Equal(4096, sec2.BlockCount);
        }

        [Fact]
        public void ExceptionTest()
        {
            var sec = ChunkSection.Build(new int[4096]);
            Assert.Throws<InvalidOperationException>(() => ChunkSection.Build(new int[0]));
            Assert.Throws<InvalidOperationException>(() => sec.SetType(ChunkSection.ChunkSectionWidth + 1, 0, 0, 0));
            Assert.Throws<InvalidOperationException>(() => sec.SetType(0, ChunkSection.ChunkSectionHeight + 1, 0, 0));
            Assert.Throws<InvalidOperationException>(() => sec.SetType(0, 0, ChunkSection.ChunkSectionWidth + 1, 0));

            var sec2 = ChunkSection.Build(Enumerable.Range(0, 4096).ToArray());
            Assert.Throws<OverflowException>(() => sec2.SetType(0, 0, 0, (int) Math.Pow(2, sec2.Data.BitsPerValue)));
        }

        [Fact]
        public void IndexTest()
        {
            Assert.Equal(0, ChunkSection.Index(0, 0, 0));
            Assert.Equal(15, ChunkSection.Index(15, 0, 0));
            Assert.Equal(16, ChunkSection.Index(0, 0, 1));
            Assert.Equal(4095, ChunkSection.Index(15, 15, 15));
            Assert.Equal(3840, ChunkSection.Index(0, 15, 0));

            Assert.Throws<InvalidOperationException>(() => ChunkSection.Index(-1, 0, 0));
            Assert.Throws<InvalidOperationException>(() => ChunkSection.Index(0, -1, 0));
            Assert.Throws<InvalidOperationException>(() => ChunkSection.Index(0, 0, -1));
            Assert.Throws<InvalidOperationException>(() => ChunkSection.Index(16, 0, 0));
            Assert.Throws<InvalidOperationException>(() => ChunkSection.Index(0, 16, 0));
            Assert.Throws<InvalidOperationException>(() => ChunkSection.Index(0, 0, 16));
        }

        [Fact]
        public void SetTypeTest()
        {
            var sec = ChunkSection.Build(new int[4096]);

            for (var x = 0; x < ChunkSection.ChunkSectionWidth; x++)
            for (var z = 0; z < ChunkSection.ChunkSectionWidth; z++)
            for (var y = 0; y < ChunkSection.ChunkSectionHeight; y++)
                sec.SetType(x, y, z, 0);

            sec.SetType(0, 0, 0, 1);
            sec.SetType(0, 0, 0, 2);
            sec.SetType(0, 0, 0, 4);

            for (var i = 0; i < 4096; i++) sec.SetType(0, 0, 0, i);
        }
    }
}