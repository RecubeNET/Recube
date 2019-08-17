using Recube.Api.Entities;
using Recube.Api.Network.Entities;
using Recube.Core.Util;

namespace Recube.Core.Entities
{
	public class PlayerRegistry : RegistryBase<Player>, IPlayerRegistry
	{
		public Player? GetPlayerByUuid(UUID uuid)
		{
			_lock.EnterReadLock();
			try
			{
				foreach (var player in _list)
				{
					if (player.Uuid.Equals(uuid))
						return player;
				}
			}
			finally
			{
				_lock.ExitReadLock();
			}

			return null;
		}
	}
}