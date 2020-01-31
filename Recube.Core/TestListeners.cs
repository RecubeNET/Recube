using System;
using System.Threading;
using Recube.Api.Event;
using Recube.Api.Event.Impl;

namespace Recube.Core
{
    // TODO REMOVE FOR PROD
    public class TestListeners
    {
        [EventListener]
        public static void OnDigging(PlayerBlockBreakEvent e)
        {
            Console.WriteLine($"CALLED IN THREAD {Thread.CurrentThread.Name}");
            e.Canceled = true;
        }
    }
}