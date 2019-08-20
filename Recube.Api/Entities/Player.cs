using System.Text;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Util;

namespace Recube.Api.Entities
{
	public class Player : Entity, IOfflinePlayer
	{
		public readonly INetworkPlayer NetworkPlayer;

		public Player(int eid, Uuid uuid, INetworkPlayer networkPlayer, string username) : base(eid)
		{
			Uuid = uuid;
			NetworkPlayer = networkPlayer;
			Username = username;
		}

		public string Username { get; }
		public Uuid Uuid { get; }

		public bool Online => NetworkPlayer.Channel.Active;

		public Player GetPlayer() => this;

		public Uuid GetOfflineUuid(string username)
		{
			return Uuid.NameUuidFromBytes(Encoding.UTF8.GetBytes("OfflinePlayer:" + username));
		}
	}
}