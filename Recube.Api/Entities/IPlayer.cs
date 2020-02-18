using System.Text;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Util;

namespace Recube.Api.Entities
{
    public interface IPlayer : IEntity, IOfflinePlayer
    {
        public INetworkPlayer NetworkPlayer { get; }
        public string Username { get; }
        public Uuid Uuid { get; }

        public bool Online => NetworkPlayer.Channel.Active;

        public Uuid GetOfflineUuid(string username)
        {
            return Uuid.NameUuidFromBytes(Encoding.UTF8.GetBytes("OfflinePlayer:" + username));
        }
    }
}