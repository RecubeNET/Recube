using Recube.Api.Entities;

namespace Recube.Api.Event
{
    public interface IPlayerEvent : IEvent
    {
        public IPlayer Player { get; }
    }
}