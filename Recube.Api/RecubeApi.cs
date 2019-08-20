using System;

namespace Recube.Api
{
	public static class RecubeApi
	{
		public static IRecube Recube { get; private set; }

		public static void SetRecubeInstance(IRecube recube)
		{
			if (Recube != null) throw new InvalidOperationException("Recube instance has already been set");

			Recube = recube;
		}
	}
}