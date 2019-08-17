using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Util;

namespace Recube.Api.Entities
{
	public class Player : Entity, IOfflinePlayer
	{
		public readonly INetworkPlayer NetworkPlayer;

		public Player(uint eid, UUID uuid, INetworkPlayer networkPlayer, string username) : base(eid)
		{
			Uuid = uuid;
			NetworkPlayer = networkPlayer;
			Username = username;
		}

		public string Username { get; }
		public UUID Uuid { get; }

		public bool Online => NetworkPlayer.Channel.Active;

		public Player GetPlayer() => this;
	}
}