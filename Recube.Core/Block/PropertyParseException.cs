using System;

namespace Recube.Core.Block
{
	class PropertyParseException : Exception
	{
		public PropertyParseException()
		{
		}

		public PropertyParseException(string message) : base(message)
		{
		}

		public PropertyParseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}