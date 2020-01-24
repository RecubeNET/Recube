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
        public readonly Action Action;

        public WorldTask(TaskCompletionSource<object> taskCompletionSource, Action action)
        {
            TaskCompletionSource = taskCompletionSource;
            Action = action;
        }
    }
}