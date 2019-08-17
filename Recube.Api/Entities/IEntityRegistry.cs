using System;

namespace Recube.Api.Entities
{
	public interface IEntityRegistry
	{
		Entity RegisterEntity(Func<uint, Entity> func);
		Entity? DeregisterEntity(uint id);
		Entity? GetEntityById(uint id);
	}
}