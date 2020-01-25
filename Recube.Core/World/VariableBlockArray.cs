﻿using System;

namespace Recube.Core.World
{
    public class VariableBlockArray
    {
        public long[] ResultingLongs { get; }
        public byte BitsPerValue { get; }
        public int Capacity { get; }
        public long Mask { get; }

        public long MaxSize() => Mask;
        
        public VariableBlockArray(byte bitsPerValue, uint capacity)
        {
            if (bitsPerValue == 0) throw new InvalidOperationException("bitsPerValue is 0");
            if (bitsPerValue > 64) throw new InvalidOperationException("bitsPerValue is greater than 64");
            if (capacity == 0) throw new InvalidOperationException("capacity is 0");

            BitsPerValue = bitsPerValue;
            Capacity = (int) capacity;
            ResultingLongs = new long[(int) Math.Ceiling(BitsPerValue * Capacity / 64d)];
            Mask = (1 << BitsPerValue) - 1;
        }

        public void Set(int index, long value)
        {
            if (index < 0) throw new InvalidOperationException($"index {index} is less than 0");
            if (index >= Capacity)
                throw new IndexOutOfRangeException($"index {index} is greater than the capacity {Capacity}");
            if (value > Mask) throw new OverflowException($"value {value} needs more bits than {BitsPerValue}");

            var startLong = index * BitsPerValue / 64;
            var startOffset = index * BitsPerValue % 64;
            var endLong =
                ((index + 1) * BitsPerValue - 1) /
                64; // CHECK NEXT BLOCK STARTING BIT BUT SUBTRACT 1 TO GET THE END LONG

            value &= Mask; // OVERFLOW PROTECTION

            ResultingLongs[startLong] |= value << startOffset;

            if (startLong != endLong)
            {
                ResultingLongs[endLong] = value >> (64 - startOffset);
            }
        }

        public long Get(int index)
        {
            if (index < 0) throw new InvalidOperationException($"index {index} is less than 0");
            if (index > Capacity) throw new InvalidOperationException($"index {index} is less than the capacity");


            var startLong = index * BitsPerValue / 64;
            var startOffset = index * BitsPerValue % 64;
            var endLong = ((index + 1) * BitsPerValue - 1) / 64;

            var value = (ulong) ResultingLongs[startLong] >> startOffset;
            if (startLong != endLong)
            {
                value |= (ulong) ResultingLongs[endLong] << (64 - startOffset);
            }

            return (long) value & Mask;
        }

        public VariableBlockArray Resize(byte bitsPerValue)
        {
            if (bitsPerValue == BitsPerValue) return this;
            if (bitsPerValue < BitsPerValue)
                throw new InvalidOperationException(
                    $"bitsPerValue {bitsPerValue} cannot be smaller than current BitsPerValue {BitsPerValue}");

            var n = new VariableBlockArray(bitsPerValue, (uint) Capacity);
            for (var i = 0; i < ResultingLongs.Length; i++)
            {
                n.Set(i, this.Get(i));
            }

            return n;
        }

        public static byte NeededBits(int number)
        {
            byte count = 0;
            do
            {
                count++;
                number >>= 1;
            } while (number != 0);

            return count;
        }
    }
}