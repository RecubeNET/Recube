using System;
using JetBrains.Annotations;

namespace Recube.Api.Event
{
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public class EventListenerAttribute : Attribute
    {
        public ListenerPriority Priority { get; }

        public EventListenerAttribute(ListenerPriority priority)
        {
            Priority = priority;
        }

        public EventListenerAttribute()
        {
            Priority = ListenerPriority.Normal;
        }
    }
}