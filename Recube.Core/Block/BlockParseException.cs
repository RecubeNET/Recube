using System;

namespace Recube.Core.Block
{
	internal class BlockParseException : Exception
	{
		public BlockParseException()
		{
		}

		public BlockParseException(string message) : base(message)
		{
		}

		public BlockParseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}