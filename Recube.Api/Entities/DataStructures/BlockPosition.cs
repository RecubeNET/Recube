namespace Recube.Api.Entities.DataStructures
{
	public struct BlockPosition
	{
		public int x;
		public int y;
		public int z;

		public BlockPosition(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public long toLong()
		{
			return (((long) x & 0x3FFFFFF) << 38) | (((long) y & 0xFFF) << 26) | ((long) z & 0x3FFFFFF);
		}
	}
}