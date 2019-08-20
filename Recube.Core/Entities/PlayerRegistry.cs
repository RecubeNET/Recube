using Recube.Api.Entities;
using Recube.Api.Util;
using Recube.Core.Util;

namespace Recube.Core.Entities
{
	public class PlayerRegistry : RegistryBase<Player>, IPlayerRegistry
	{
		public Player? GetPlayerByUuid(Uuid uuid)
		{
			Lock.EnterReadLock();
			try
			{
				foreach (var player in List)
				{
					if (player.Uuid.Equals(uuid))
						return player;
				}
			}
			finally
			{
				Lock.ExitReadLock();
			}

			return null;
		}
	}
}