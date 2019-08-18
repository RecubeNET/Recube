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

		public static NbtString? GetString(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtString>(tag);
		}

		public static NbtCompound AddByte(this NbtCompound nbt, string tag, byte value)
		{
			nbt[tag] = new NbtByte(tag, value);
			return nbt;
		}

		public static NbtByte? GetByte(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtByte>(tag);
		}

		public static NbtCompound AddDouble(this NbtCompound nbt, string tag, double value)
		{
			nbt[tag] = new NbtDouble(tag, value);
			return nbt;
		}

		public static NbtDouble? GetDouble(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtDouble>(tag);
		}

		public static NbtCompound AddLong(this NbtCompound nbt, string tag, long value)
		{
			nbt[tag] = new NbtLong(tag, value);
			return nbt;
		}

		public static NbtLong? GetLong(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtLong>(tag);
		}

		public static NbtCompound AddInt(this NbtCompound nbt, string tag, int value)
		{
			nbt[tag] = new NbtInt(tag, value);
			return nbt;
		}

		public static NbtInt? GetInt(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtInt>(tag);
		}

		public static NbtCompound AddNbtCompound(this NbtCompound nbt, NbtCompound value)
		{
			nbt[value.Name ?? throw new NullReferenceException("NbtCompound name can't be null")] = value;
			return nbt;
		}

		public static NbtCompound? GetNbtCompound(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtCompound>(tag);
		}
	}
}