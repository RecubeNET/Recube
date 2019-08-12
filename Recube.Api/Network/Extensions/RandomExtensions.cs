using System;

namespace Recube.Api.Network.Extensions
{
	public static class RandomExtensions
	{
		public static long RandomLong(this Random rnd)
		{
			var buffer = new byte[8];
			rnd.NextBytes(buffer);
			return BitConverter.ToInt64(buffer, 0);
		}
	}
}