using System;

namespace Recube.Core.Block
{
	class FileParseException : Exception
	{
		public FileParseException()
		{
		}

		public FileParseException(string message) : base(message)
		{
		}

		public FileParseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}