using DotNetty.Transport.Channels;
using Recube.Core.Util;

namespace Recube.Core.Network.NetworkPlayer
{
	public class NetworkPlayerRegistry : RegistryBase<NetworkPlayer>
	{
		public NetworkPlayer? GetNetworkPlayerByChannel(IChannel channel)
		{
			_lock.EnterReadLock();
			try
			{
				foreach (var networkPlayer in _list)
				{
					if (!networkPlayer.Channel.Equals(channel)) continue;
					return networkPlayer;
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