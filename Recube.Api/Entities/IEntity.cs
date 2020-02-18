using Recube.Api.World;

namespace Recube.Api.Entities
{
    public interface IEntity
    {
        public int EntityId { get; }

        public IWorld World { get; }
    }
}