using System;
using fNbt;

namespace Recube.Api.Network.Extensions
{
	public static class NbtCompoundExtensions
	{
		public static NbtCompound AddString(this NbtCompound nbt, string tag, string value)
		{
			nbt[tag] = new NbtString(tag, value);
			return nbt;
		}

		public static NbtCompound AddByte(this NbtCompound nbt, string tag, byte value)
		{
			nbt[tag] = new NbtByte(tag, value);
			return nbt;
		}

		public static NbtCompound AddDouble(this NbtCompound nbt, string tag, double value)
		{
			nbt[tag] = new NbtDouble(tag, value);
			return nbt;
		}

		public static NbtCompound AddLong(this NbtCompound nbt, string tag, long value)
		{
			nbt[tag] = new NbtLong(tag, value);
			return nbt;
		}

		public static NbtCompound AddInt(this NbtCompound nbt, string tag, int value)
		{
			nbt[tag] = new NbtInt(tag, value);
			return nbt;
		}

		public static NbtCompound AddNbtCompound(this NbtCompound nbt, NbtCompound value)
		{
			nbt[value.Name ?? throw new NullReferenceException("NbtCompound can't be null")] = value;
			return nbt;
		}
	}
}