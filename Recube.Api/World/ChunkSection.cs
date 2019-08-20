using Recube.Api.Block;
using Recube.Api.Block.Impl;
using Recube.Api.World.Paletts;

namespace Recube.Api.World
{
	public class ChunkSection
	{
		public BaseBlock[,,] Blocks = new BaseBlock[16, 16, 16];
		public IPalette Palette;

		public ChunkSection()
		{
			Palette = ChoosePalette(8);
		}

		public IPalette ChoosePalette(byte bitsPerBlock)
		{
			if (bitsPerBlock <= 4)
			{
				return new IndirectPalette(4);
			}

			if (bitsPerBlock <= 8)
			{
				return new IndirectPalette(bitsPerBlock);
			}

			return new DirectPalette();
		}

		public BaseBlock GetBaseBlock(in int x, in int y, in int z)
		{
			//TODO: Implement
			//return blocks[x, y, z];
			return new AirBlock();
		}

		public byte GetBlockLight(in int x, in int y, in int z)
		{
			//TODO: Implement
			return 0b0000;
		}

		public byte GetSkyLight(in int x, in int y, in int z)
		{
			//TODO: Implement
			return 0b0000;
		}
	}
}