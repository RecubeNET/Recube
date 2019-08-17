using System;
using DotNetty.Buffers;
using Recube.Api.Block;

namespace Recube.Core.World.Paletts
{
	public class DirectPalette : Palette
	{
		public uint IdForState(BlockState state)
		{
			return BlockState.GetGlobalPaletteIDFromState(state);
		}

		public BlockState StateForId(uint id)
		{
			return BlockState.GetStateFromGlobalPaletteID(id);
		}

		public byte GetBitsPerBlock()
		{
			return (byte) MathF.Ceiling(MathF.Log2(BlockState.TotalNumberOfStates)); // currently 14
		}

		public void Read(IByteBuffer data)
		{
			// No Data
		}

		public void Write(IByteBuffer data)
		{
			// No Data
		}
	}
}