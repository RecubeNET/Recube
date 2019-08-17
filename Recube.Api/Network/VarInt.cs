using System.Collections.Generic;
using DotNetty.Buffers;

namespace Recube.Api.Network
{
	public static class VarInt
	{
		/// <summary>
		///     Reads a VarInt defined by the Minecraft protocol
		/// </summary>
		/// <param name="buffer">The buffer</param>
		/// <param name="varInt">The parsed VarInt. If the bytes could not be successfully parsed, the VarInt is null</param>
		/// <returns>True if the bytes have been parsed successfully. False otherwise</returns>
		public static bool ReadVarInt(IByteBuffer buffer, out int? varInt)
		{
			varInt = null;
			var result = 0;
			var bytesRead = 0;
			byte nextByte;

			do
			{
				nextByte = buffer.ReadByte();
				var value = nextByte & 0b0111_1111;
				result |= value << (7 * bytesRead);
				bytesRead++;

				if (bytesRead > 5) return false;
			} while ((nextByte & 0b1000_0000) != 0);

			varInt = result;
			return true;
		}

		/// <summary>
		///     Writes a VarInt defined by the Minecraft protocol
		/// </summary>
		/// <param name="number">The int to write</param>
		/// <param name="bytes">The parsed VarInt</param>
		public static void WriteVarInt(int number, out byte[] bytes)
		{
			var list = new List<byte>();
			do
			{
				var temp = (byte) (number & 0b0111_1111);
				number = (int) ((uint) number >> 7);

				if (number != 0) // Append a 1 if there are more bytes
					temp |= 0b1000_0000;

				list.Add(temp);
			} while (number != 0);

			bytes = list.ToArray();
			list.Clear();
		}

		/// <summary>
		///     Reads a VarLong defined by the Minecraft protocol
		/// </summary>
		/// <param name="buffer">The buffer</param>
		/// <param name="varLong">The parsed VarLong. If the bytes could not be successfully parsed, the VarInt is null</param>
		/// <returns>True if the bytes have been parsed successfully. False otherwise</returns>
		public static bool ReadVarLong(IByteBuffer buffer, out long? varLong)
		{
			varLong = null;
			var result = 0;
			var bytesRead = 0;
			byte nextByte;

			do
			{
				nextByte = buffer.ReadByte();
				var value = nextByte & 0b0111_1111;
				result |= value << (7 * bytesRead);
				bytesRead++;

				if (bytesRead > 10) return false;
			} while ((nextByte & 0b1000_0000) != 0);

			varLong = result;
			return true;
		}

		/// <summary>
		///     Writes a VarLong defined by the Minecraft protocol
		/// </summary>
		/// <param name="number">The long to write</param>
		/// <param name="bytes">The parsed VarLong</param>
		public static void WriteVarLong(long number, out byte[] bytes)
		{
			var list = new List<byte>();
			do
			{
				var temp = (byte) (number & 0b0111_1111);
				number = (int) ((uint) number >> 7);

				if (number != 0) // Append a 1 if there are more bytes
					temp |= 0b1000_0000;

				list.Add(temp);
			} while (number != 0);

			bytes = list.ToArray();
			list.Clear();
		}
	}
}