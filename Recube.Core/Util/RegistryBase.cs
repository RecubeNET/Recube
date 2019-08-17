using System.Collections.Generic;
using System.Threading;
using Recube.Api.Util;

namespace Recube.Core.Util
{
	public class RegistryBase<T> : IRegistryBase<T>
	{
		protected List<T> _list = new List<T>();
		protected ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		public void Register(T t)
		{
			_lock.EnterWriteLock();
			try
			{
				_list.Add(t);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public void Deregister(T t)
		{
			_lock.EnterWriteLock();
			try
			{
				_list.Remove(t);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public IList<T> GetAll()
		{
			_lock.EnterReadLock();
			try
			{
				return new List<T>(_list);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}
	}
}