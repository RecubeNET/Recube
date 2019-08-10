using System.Collections.Generic;

namespace Recube.Api.Util
{
	public interface IRegistryBase<T>
	{
		void Register(T t);

		void Deregister(T t);

		IList<T> GetAll();
	}
}