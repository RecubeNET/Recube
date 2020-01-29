using Recube.Api.Entities;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Util;
using Recube.Api.World;

namespace Recube.Core.Entities
{
    public class Player : IPlayer
    {
        public INetworkPlayer NetworkPlayer { get; }
        public Uuid Uuid { get; }
        public int EntityId { get; set; }
        public string Username { get; }

        public IWorld World { get; set; }

        public bool Online { get; set; }


        public Player(int eid, Uuid uuid, INetworkPlayer networkPlayer, string username)
        {
            Uuid = uuid;
            NetworkPlayer = networkPlayer;
            Username = username;
        }


        public IPlayer? GetPlayer()
        {
            throw new System.NotImplementedException();
        }
    }
}