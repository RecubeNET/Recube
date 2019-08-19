using System.Linq;

namespace Recube.Api.Network.Extensions
{
	public static class ByteArrayExtention
	{
		public static byte[] ToBigEndian(this byte[] bytes)
		{
			return bytes.Reverse().ToArray();
		}
	}
}