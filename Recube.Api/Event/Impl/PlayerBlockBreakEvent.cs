using Recube.Api.Entities;
using Recube.Api.World;

namespace Recube.Api.Event.Impl
{
    public class PlayerBlockBreakEvent : IWorldPlayerEvent, ICancelable
    {
        public IWorld World { get; }
        public IPlayer Player { get; }

        public bool Canceled { get; set; }

        // TODO ADD LOCATION
        public PlayerBlockBreakEvent(IWorld world, IPlayer player)
        {
            World = world;
            Player = player;
        }
    }
}