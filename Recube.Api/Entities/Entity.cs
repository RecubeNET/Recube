namespace Recube.Api.Entities
{
	public abstract class Entity
	{
		public readonly int EntityId;


		public Entity(int entityId)
		{
			EntityId = entityId;
		}
	}
}