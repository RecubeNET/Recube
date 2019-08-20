using System;
using DotNetty.Buffers;
using Recube.Api.Block;

namespace Recube.Api.World.Paletts
{
	public class DirectPalette : IPalette
	{
		public uint IdForState(BlockState state)
		{
			//TODO: Implement
			return BlockState.GetGlobalPaletteIDFromState(state);
		}

		public BlockState StateForId(uint id)
		{
			//TODO: Implement
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