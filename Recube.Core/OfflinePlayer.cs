using Recube.Api;
using Recube.Api.Entities;
using Recube.Api.Util;

namespace Recube.Core
{
	public class OfflinePlayer : IOfflinePlayer
	{
		public OfflinePlayer(UUID uuid)
		{
			Uuid = uuid;
		}

		public UUID Uuid { get; }
		public bool Online => GetPlayer() != null;

		public Player GetPlayer()
		{
			return Recube.Instance.PlayerRegistry.GetPlayerByUuid(Uuid);
		}
	}
}