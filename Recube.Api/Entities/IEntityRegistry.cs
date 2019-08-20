using System;

namespace Recube.Api.Entities
{
	public interface IEntityRegistry
	{
		Entity RegisterEntity(Func<int, Entity> func);
		Entity? DeregisterEntity(int id);
		Entity? GetEntityById(int id);
	}
}