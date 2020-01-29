using System;

namespace Recube.Api.Event
{
    public interface IListenerRegistry
    {
        public void RegisterAll(Type listener);

        public void DeregisterAll(Type listener);

        public void Fire(IEvent e);
    }
}