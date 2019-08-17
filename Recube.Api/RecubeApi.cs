using System;
using Recube.Core.Block;

namespace Recube.Api
{
	public static class RecubeApi
	{
		public static IRecube Recube { get; private set; }
		public static BlockStateRegistry BlockStateRegistry { get; private set; }

		public static void SetRecubeInstance(IRecube recube)
		{
			if (Recube != null) throw new InvalidOperationException("Recube instance has already been set");
			BlockStateRegistry = new BlockStateRegistry();
			Recube = recube;
		}
	}
}