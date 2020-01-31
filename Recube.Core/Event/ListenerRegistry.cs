using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using NLog;
using Recube.Api.Event;

namespace Recube.Core.Event
{
    public class ListenerRegistry : IListenerRegistry
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        // EVENT TYPE, LIST<LISTENER>
        private Dictionary<Type, List<MethodInfo>> _listeners = new Dictionary<Type, List<MethodInfo>>();
        private ILogger _logger = LogManager.GetLogger("ListenerRegistry");

        public void RegisterAll(Type listener)
        {
            foreach (var method in listener.GetMethods())
            {
                var attr = method.GetCustomAttribute<EventListenerAttribute>(false);
                if (attr == null) continue;

                if (!method.IsStatic)
                {
                    _logger.Warn(
                        $"Tried to add an even listener {listener.FullName}#{method.Name} which is not static");
                    continue;
                }

                var ev = (ParameterInfo?) method.GetParameters().GetValue(0);
                if (ev == null)
                {
                    _logger.Warn(
                        $"Tried to add an even listener {listener.FullName}#{method.Name} without an event parameter");
                    continue;
                }

                ;
                var type = ev.ParameterType;

                _lock.EnterWriteLock();

                try
                {
                    _logger.Info($"Adding event listener {listener.FullName}#{method.Name} ({type.FullName})");
                    if (_listeners.TryAdd(type, new List<MethodInfo> {method})) continue;
                    _listeners.TryGetValue(type, out var inList);
                    inList?.Add(method);
                    inList = inList?.OrderBy(m =>
                    {
                        var at = m.GetCustomAttribute<EventListenerAttribute>();
                        return (int) (at?.Priority ?? ListenerPriority.Normal);
                    }).ToList();
                    _listeners[type] = inList;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public void DeregisterAll(Type listener)
        {
            foreach (var method in listener.GetMethods())
            {
                var attr = method.GetCustomAttribute<EventListenerAttribute>(false);
                if (attr == null) continue;

                var ev = (ParameterInfo?) method.GetParameters().GetValue(0);
                if (ev == null) continue;
                var type = ev.ParameterType;

                _lock.EnterWriteLock();
                try
                {
                    _listeners.TryGetValue(type, out var list);
                    _logger.Info($"Removing event listener {listener.FullName}#{method.Name} ({type.FullName})");
                    list?.Remove(method);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public void Fire(IEvent e)
        {
            _logger.Debug($"Firing event {e.GetType().FullName}");
            var type = e.GetType();
            var methods = new List<MethodInfo>();

            _lock.EnterReadLock();
            try
            {
                if (!_listeners.TryGetValue(type, out var val)) return;
                methods.AddRange(val);
            }
            finally
            {
                _lock.ExitReadLock();
            }

            foreach (var m in methods)
            {
                m.Invoke(null, new object[] {e});
                if (!(e is ICancelable cancelable)) continue;

                if (cancelable.Canceled)
                    break;
            }
        }
    }
}