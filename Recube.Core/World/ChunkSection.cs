using System.Collections.Generic;
using Recube.Api.Block;
using Recube.Core.World.Paletts;

namespace Recube.Core.World
{
	public class ChunkSection
	{
		public Palette Palette;

		public ChunkSection()
		{
			Palette = ChoosePalette(0);
		}

		public Palette ChoosePalette(byte bitsPerBlock)
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

		public BlockState GetState(in int x, in int y, in int z)
		{
			//TODO: Implement
			return new BlockState(0, true, new Dictionary<string, object>());
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