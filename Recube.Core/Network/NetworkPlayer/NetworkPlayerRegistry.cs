using DotNetty.Transport.Channels;
using Recube.Core.Util;

namespace Recube.Core.Network.NetworkPlayer
{
	public class NetworkPlayerRegistry : RegistryBase<NetworkPlayer>
	{
		public NetworkPlayer? GetNetworkPlayerByChannel(IChannel channel)
		{
			Lock.EnterReadLock();
			try
			{
				foreach (var networkPlayer in List)
				{
					if (!networkPlayer.Channel.Equals(channel)) continue;
					return networkPlayer;
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