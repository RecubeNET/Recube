using System;
using System.Collections.Generic;
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

		public static string GetString(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtString>(tag).Value;
		}

		public static NbtCompound AddByte(this NbtCompound nbt, string tag, byte value)
		{
			nbt[tag] = new NbtByte(tag, value);
			return nbt;
		}

		public static byte GetByte(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtByte>(tag).Value;
		}

		public static NbtCompound AddDouble(this NbtCompound nbt, string tag, double value)
		{
			nbt[tag] = new NbtDouble(tag, value);
			return nbt;
		}

		public static double GetDouble(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtDouble>(tag).Value;
		}

		public static NbtCompound AddLong(this NbtCompound nbt, string tag, long value)
		{
			nbt[tag] = new NbtLong(tag, value);
			return nbt;
		}

		public static long GetLong(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtLong>(tag).Value;
		}

		public static NbtCompound AddInt(this NbtCompound nbt, string tag, int value)
		{
			nbt[tag] = new NbtInt(tag, value);
			return nbt;
		}

		public static int GetInt(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtInt>(tag).Value;
		}

		public static NbtCompound AddNbtCompound(this NbtCompound nbt, NbtCompound value)
		{
			nbt[value.Name ?? throw new NullReferenceException("NbtCompound name can't be null")] = value;
			return nbt;
		}

		public static NbtCompound GetNbtCompound(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtCompound>(tag);
		}

		public static NbtCompound AddBoolean(this NbtCompound nbt, string tag, bool value)
		{
			nbt.AddByte(tag, value ? (byte) 1 : (byte) 0);
			return nbt;
		}

		public static bool GetBoolean(this NbtCompound nbt, string tag)
		{
			// ReSharper disable once PossibleNullReferenceException
			if (nbt.Get<NbtByte>(tag) != null && nbt.Get<NbtByte>(tag).Value == 1)
			{
				return true;
			}

			return false;
		}

		public static NbtCompound AddNbtCompoundArray(this NbtCompound nbt, string tag, List<NbtCompound> value)
		{
			nbt[tag] = new NbtList(tag, value, NbtTagType.Compound);
			return nbt;
		}

		public static NbtList GetNbtCompoundArray(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtList>(tag);
		}

		public static NbtCompound AddByteArray(this NbtCompound nbt, string tag, byte[] value)
		{
			nbt[tag] = new NbtByteArray(tag, value);
			return nbt;
		}

		public static byte[] GetByteArray(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtByteArray>(tag).Value;
		}

		public static NbtCompound AddIntArray(this NbtCompound nbt, string tag, int[] value)
		{
			nbt[tag] = new NbtIntArray(tag, value);
			return nbt;
		}

		public static int[] GetIntArray(this NbtCompound nbt, string tag)
		{
			return nbt.Get<NbtIntArray>(tag).Value;
		}
	}
}