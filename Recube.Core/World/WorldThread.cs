using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Recube.Core.World
{
    /// <summary>
    /// This class manages worlds. It keeps them ticking and synchronized with <see cref="WorldTask"/>s.
    /// 
    /// </summary>
    public class WorldThread : IDisposable
    {
        private readonly ILogger _logger;
        private readonly List<World> _worlds = new List<World>();
        private readonly ConcurrentQueue<WorldTask> _tasks = new ConcurrentQueue<WorldTask>();
        private readonly Thread _thread;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private ulong _timeCatchup = 0;
        private bool _running = false;

        public bool IsRunning() => _running;

        public WorldThread(uint id)
        {
            _logger = LogManager.GetLogger($"World Thread #{id}");
            _thread = new Thread(ThreadRun) {Name = $"World Thread #{id}"};
        }

        /// <summary>
        /// Adds an world to this thread. The thread will take care of synchronizing and ticking.
        /// </summary>
        /// <param name="world">The world</param>
        public void AddWorld(World world)
        {
            lock (_worlds)
            {
                _logger.Info($"Adding world {world.Name}");
                _worlds.Add(world);
                world.CurrentWorldThread = this;
            }
        }

        /// <summary>
        /// Removes this world from the list so that this thread won't trigger the tick for this world anymore.
        /// To unload & save the world use World.<see cref="World.Dispose"/>
        /// </summary>
        /// <param name="world">The world</param>
        public void RemoveWorld(World world)
        {
            lock (_worlds)
            {
                _logger.Info($"Removing world {world.Name}");
                _worlds.Remove(world);
            }
        }

        /// <summary>
        /// Starts the world thread
        /// </summary>
        /// <exception cref="InvalidOperationException">If the thread is already running</exception>
        public void Start()
        {
            if (_thread.IsAlive || _running) throw new InvalidOperationException("thread already started");
            lock (_worlds)
            {
                _logger.Info($"Starting world thread with {_worlds.Count} worlds");
            }

            _thread.Start();
            _running = true;
        }

        /// <summary>
        /// Stops the thread and call World.<see cref="World.Dispose"/> on every world
        /// </summary>
        public void Stop()
        {
            _running = false;
        }

        /// <summary>
        /// Runs the given action in the world thread
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>An awaitable task</returns>
        public Task Execute(Func<Task> action)
        {
            var tcs = new TaskCompletionSource<object>();
            _tasks.Enqueue(new WorldTask(tcs, action));
            return tcs.Task;
        }

        private void ThreadRun()
        {
            var worldsToRemove = new List<World>();
            while (_running)
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                lock (_worlds)
                {
                    foreach (var world in _worlds)
                    {
                        try
                        {
                            world.Tick();
                        }
                        catch (Exception e)
                        {
                            _logger.Error($"World {world.Name} raised an exception. Disabling world...: {e}");
                            worldsToRemove.Add(world);
                        }
                    }

                    while (_tasks.TryDequeue(out var worldTask))
                    {
                        try
                        {
                            worldTask.Action.Invoke().Wait(); // AWAIT
                            worldTask.TaskCompletionSource.SetResult(null);
                        }
                        catch (Exception e)
                        {
                            worldTask.TaskCompletionSource.SetException(e);
                        }
                    }
                }


                // REMOVE WORLDS THAT RAISED AN EXCEPTION
                if (worldsToRemove.Count != 0)
                {
                    foreach (var world in worldsToRemove)
                    {
                        RemoveWorld(world);
                    }

                    worldsToRemove.Clear();
                }

                // CALCULATE HOW LONG WE NEED TO SLEEP
                // IF WE NEEDED LONGER THAN 50 MILLISECONDS, WE WANT TO CATCHUP TO THE 20 TPS

                _stopwatch.Stop();
                var overtime = _stopwatch.ElapsedMilliseconds - 50;
                if (overtime > 0) _timeCatchup += (ulong) overtime;

                var sleepTime = 50;
                var sub = Math.Min(50, _timeCatchup);
                _timeCatchup -= sub;

                Thread.Sleep(sleepTime - (int) sub);
            }

            lock (_worlds)
            {
                foreach (var world in _worlds)
                {
                    world.Dispose();
                }
            }
        }

        public void Dispose() => Stop();
    }
}