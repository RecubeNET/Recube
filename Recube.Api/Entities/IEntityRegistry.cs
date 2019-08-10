namespace Recube.Api.Entities
{
	public interface IEntityRegistry
	{
		uint RegisterEntity(Entity entity);
		Entity? DeregisterEntity(uint id);
		Entity? GetEntityById(uint id);
	}
}