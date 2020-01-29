using Recube.Api.World;

namespace Recube.Api.Event
{
    public interface IWorldEvent : IEvent
    {
        public IWorld World { get; }
    }
}