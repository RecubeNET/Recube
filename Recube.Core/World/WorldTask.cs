using System;
using System.Threading.Tasks;

namespace Recube.Core.World
{
    /// <summary>
    /// Just a helper struct for executing code in a <see cref="WorldThread"/>.
    /// </summary>
    public struct WorldTask
    {
        public readonly TaskCompletionSource<object> TaskCompletionSource;
        public readonly Func<Task> Action;

        public WorldTask(TaskCompletionSource<object> taskCompletionSource, Func<Task> action)
        {
            TaskCompletionSource = taskCompletionSource;
            Action = action;
        }
    }
}