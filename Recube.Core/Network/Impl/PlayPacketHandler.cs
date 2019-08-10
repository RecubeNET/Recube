using System.Threading.Tasks;
using Recube.Api.Entities;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;
using Recube.Api.Util;

namespace Recube.Core.Network.Impl
{
	public class PlayPacketHandler : PacketHandler
	{
		private Player _player;

		public PlayPacketHandler(INetworkPlayer networkPlayer) : base(networkPlayer)
		{
		}

		public override void OnActive()
		{
			((NetworkPlayer.NetworkPlayer) NetworkPlayer).SetState(NetworkPlayerState.Login);
		}

		public override void OnDisconnect()
		{
			if (_player == null) return;

			Recube.Instance.PlayerRegistry.Deregister(_player);
			_player.Remove();
			NetworkBootstrap.Logger.Info($"Player {_player.Username}[{_player.Uuid}] disconnected");
		}

		public override Task Fallback(IInPacket packet)
		{
			return Task.CompletedTask;
		}

		internal void SetPlayer(UUID uuid, string username)
		{
			_player = new Player(uuid, (NetworkPlayer.NetworkPlayer) NetworkPlayer, username);
			Recube.Instance.PlayerRegistry.Register(_player);
			NetworkBootstrap.Logger.Info(
				$"Player {_player.Username}[{_player.Uuid}] connected from {NetworkPlayer.Channel.RemoteAddress}");
		}
	}
}