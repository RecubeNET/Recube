// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace Recube.Api.Entities.DataStructures
{
	public struct BlockPosition
	{
		public int X;
		public int Y;
		public int Z;

		public BlockPosition(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public long ToLong()
		{
			return (((long) X & 0x3FFFFFF) << 38) | (((long) Y & 0xFFF) << 26) | ((long) Z & 0x3FFFFFF);
		}
	}
}