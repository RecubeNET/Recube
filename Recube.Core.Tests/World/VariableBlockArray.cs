using System;
using Xunit;
using Xunit.Abstractions;

namespace Recube.Core.Tests.World
{
    public class VariableBlockArray
    {
        [Fact]
        public void SetTest()
        {
            var vba = new Core.World.VariableBlockArray(14, 5);
            var mask = (1 << 14) - 1;

            vba.Set(0, 200);
            vba.Set(1, 5000);
            vba.Set(2, 1);
            vba.Set(3, 4238);
            vba.Set(4, 12000);

            var arr = vba.ResultingLongs;
            Assert.Equal(new[] {-2287204087749279544, 46}, arr);
        }

        [Fact]
        public void SetOverflowExceptionTest()
        {
            var vba = new Core.World.VariableBlockArray(14, 1);
            Assert.Throws<OverflowException>(() => vba.Set(0, 16384));
        }
        
        [Fact]
        public void SetIndexOutOfRangeExceptionTest()
        {
            var vba = new Core.World.VariableBlockArray(14, 1);
            Assert.Throws<IndexOutOfRangeException>(() => vba.Set(1, 220));
        }

        [Fact]
        public void InvalidOperationExceptionTest()
        {
            Assert.Throws<InvalidOperationException>(() => new Core.World.VariableBlockArray(0, 1));
            Assert.Throws<InvalidOperationException>(() => new Core.World.VariableBlockArray(65, 1));
            Assert.Throws<InvalidOperationException>(() => new Core.World.VariableBlockArray(64, 0));
        }

        [Fact]
        public void GetTest()
        {
            var vba = new Core.World.VariableBlockArray(14, 5);
            vba.Set(0, 200);
            vba.Set(1, 5000);
            vba.Set(2, 1);
            vba.Set(3, 4238);
            vba.Set(4, 12000);
            
            Assert.Equal(200, vba.Get(0));
            Assert.Equal(5000, vba.Get(1));
            Assert.Equal(1, vba.Get(2));
            Assert.Equal(4238, vba.Get(3));
            Assert.Equal(12000, vba.Get(4));
        }

        [Fact]
        public void GetTest2()
        {
            var bits = Core.World.VariableBlockArray.NeededBits(int.MaxValue);
            var vba = new Core.World.VariableBlockArray(bits, 6);
            vba.Set(0, 200);
            vba.Set(1, 5000);
            vba.Set(2, 1);
            vba.Set(3, 4238);
            vba.Set(4, 12000);
            vba.Set(5, 1200000);
            
            Assert.Equal(200, vba.Get(0));
            Assert.Equal(5000, vba.Get(1));
            Assert.Equal(1, vba.Get(2));
            Assert.Equal(4238, vba.Get(3));
            Assert.Equal(12000, vba.Get(4));
            Assert.Equal(1200000, vba.Get(5));
        }

        [Fact]
        public void NeededBitsTest()
        {
            const int number1 = 0; // 1 bit
            const int number2 = 1; // 1 bit
            const int number3 = 0x3F; // 6 bits
            const int number4 = 0x6FF; // 11 bits
            const int number5 = 0xFFFFFF; // 24 bits

            Assert.Equal(1u, Core.World.VariableBlockArray.NeededBits(number1));
            Assert.Equal(1u, Core.World.VariableBlockArray.NeededBits(number2));
            Assert.Equal(6u, Core.World.VariableBlockArray.NeededBits(number3));
            Assert.Equal(11u, Core.World.VariableBlockArray.NeededBits(number4));
            Assert.Equal(24u, Core.World.VariableBlockArray.NeededBits(number5));
        }
    }
}