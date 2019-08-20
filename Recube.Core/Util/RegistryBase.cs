using System.Collections.Generic;
using System.Threading;
using Recube.Api.Util;

namespace Recube.Core.Util
{
	public class RegistryBase<T> : IRegistryBase<T>
	{
		protected List<T> List = new List<T>();
		protected ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

		public void Register(T t)
		{
			Lock.EnterWriteLock();
			try
			{
				List.Add(t);
			}
			finally
			{
				Lock.ExitWriteLock();
			}
		}

		public void Deregister(T t)
		{
			Lock.EnterWriteLock();
			try
			{
				List.Remove(t);
			}
			finally
			{
				Lock.ExitWriteLock();
			}
		}

		public IList<T> GetAll()
		{
			Lock.EnterReadLock();
			try
			{
				return new List<T>(List);
			}
			finally
			{
				Lock.ExitReadLock();
			}
		}
	}
}