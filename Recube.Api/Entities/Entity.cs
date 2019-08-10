namespace Recube.Api.Entities
{
	public abstract class Entity
	{
		public readonly uint EntityId;

		public Entity()
		{
			EntityId = RecubeApi.Recube.GetEntityRegistry().RegisterEntity(this);
		}

		public void Remove()
		{
			RecubeApi.Recube.GetEntityRegistry().DeregisterEntity(EntityId);
		}
	}
}