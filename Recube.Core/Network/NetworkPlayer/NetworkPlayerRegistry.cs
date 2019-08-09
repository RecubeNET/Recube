using System.Collections.Generic;
using System.Threading;
using DotNetty.Transport.Channels;

namespace Recube.Core.Network.NetworkPlayer
{
	public class NetworkPlayerRegistry
	{
		private List<NetworkPlayer> _list = new List<NetworkPlayer>();
		private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		public void Register(NetworkPlayer player)
		{
			_lock.EnterWriteLock();
			try
			{
				_list.Add(player);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public void Deregister(NetworkPlayer player)
		{
			_lock.EnterWriteLock();
			try
			{
				_list.Remove(player);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

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

		public IList<NetworkPlayer> GetNetworkPlayers()
		{
			_lock.EnterReadLock();
			try
			{
				return new List<NetworkPlayer>(_list);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}
	}
}