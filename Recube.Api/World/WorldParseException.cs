using System;

namespace Recube.Api.World
{
	internal class WorldParseException : Exception
	{
		public WorldParseException()
		{
		}

		public WorldParseException(string message) : base(message)
		{
		}

		public WorldParseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}