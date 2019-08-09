using System;

namespace Recube.Core.Network
{
	public class BootstrapConnectionClosedException : Exception
	{
		public BootstrapConnectionClosedException()
		{
		}

		public BootstrapConnectionClosedException(string message) : base(message)
		{
		}

		public BootstrapConnectionClosedException(string message, Exception innerException) : base(message,
			innerException)
		{
		}
	}
}