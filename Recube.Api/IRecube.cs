using NLog;
using Recube.Api.Entities;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api
{
	public interface IRecube
	{
		ILogger Logger { get; }

		IPacketRegistry GetCorrectPacketRegistry(NetworkPlayerState state, PacketDirection direction);

		IPlayerRegistry GetPlayerRegistry();
		IEntityRegistry GetEntityRegistry();
	}
}