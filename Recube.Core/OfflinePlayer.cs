using Recube.Api;
using Recube.Api.Entities;
using Recube.Api.Util;

namespace Recube.Core
{
    public class OfflinePlayer : IOfflinePlayer
    {
        public OfflinePlayer(Uuid uuid)
        {
            Uuid = uuid;
        }

        public Uuid Uuid { get; }
        public bool Online => GetPlayer() != null;

        public IPlayer? GetPlayer()
        {
            return Recube.Instance.PlayerRegistry.GetPlayerByUuid(Uuid);
        }
    }
}