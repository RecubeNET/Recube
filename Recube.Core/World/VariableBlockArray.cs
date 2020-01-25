using System;

namespace Recube.Core.World
{
    public class VariableBlockArray
    {
        public long[] ResultingLongs { get; }
        private readonly int _bitsPerValue;
        private readonly int _capacity;
        private readonly long _mask;

        public VariableBlockArray(uint bitsPerValue, uint capacity)
        {
            if (bitsPerValue == 0) throw new InvalidOperationException("bitsPerValue is 0");
            if (bitsPerValue > 64) throw new InvalidOperationException("bitsPerValue is greater than 64");
            if (capacity == 0) throw new InvalidOperationException("capacity is 0");

            _bitsPerValue = (int) bitsPerValue;
            _capacity = (int) capacity;
            ResultingLongs = new long[(int) Math.Ceiling(_bitsPerValue * _capacity / 64d)];
            _mask = (1 << _bitsPerValue) - 1;
        }

        public void Set(int index, long value)
        {
            if (index < 0) throw new InvalidOperationException($"index {index} is less than 0");
            if (index >= _capacity)
                throw new IndexOutOfRangeException($"index {index} is greater than the capacity {_capacity}");
            if (value > _mask) throw new OverflowException($"value {value} needs more bits than {_bitsPerValue}");

            var startLong = index * _bitsPerValue / 64;
            var startOffset = index * _bitsPerValue % 64;
            var endLong =
                ((index + 1) * _bitsPerValue - 1) /
                64; // CHECK NEXT BLOCK STARTING BIT BUT SUBTRACT 1 TO GET THE END LONG

            value &= _mask; // OVERFLOW PROTECTION

            ResultingLongs[startLong] |= value << startOffset;

            if (startLong != endLong)
            {
                ResultingLongs[endLong] = value >> (64 - startOffset);
            }
        }

        public long Get(int index)
        {
            if (index < 0) throw new InvalidOperationException($"index {index} is less than 0");
            if (index > _capacity) throw new InvalidOperationException($"index {index} is less than the capacity");


            var startLong = index * _bitsPerValue / 64;
            var startOffset = index * _bitsPerValue % 64;
            var endLong = ((index + 1) * _bitsPerValue - 1) / 64;

            var value = (ulong) ResultingLongs[startLong] >> startOffset;
            if (startLong != endLong)
            {
                value |= (ulong) ResultingLongs[endLong] << (64 - startOffset);
            }

            return (long) value & _mask;
        }

        public long MaxSize()
        {
            return _mask;
        }

        public VariableBlockArray Resize(uint bitsPerValue)
        {
            if (bitsPerValue == _bitsPerValue) return this;
            if (bitsPerValue < _bitsPerValue)
                throw new InvalidOperationException(
                    $"bitsPerValue {bitsPerValue} cannot be smaller than current _bitsPerValue {_bitsPerValue}");
            
            var n = new VariableBlockArray(bitsPerValue, (uint) _capacity);
            for (var i = 0; i < ResultingLongs.Length; i++)
            {
                n.Set(i, this.Get(i));
            }

            return n;
        }

        public static uint NeededBits(int number)
        {
            var count = 0u;
            do
            {
                count++;
                number >>= 1;
            } while (number != 0);

            return count;
        }
    }
}