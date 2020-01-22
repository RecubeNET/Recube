using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;

namespace Recube.Api.Network.NetworkPlayer
{
	public interface INetworkPlayer
	{
		IChannel Channel { get; }
		PacketHandler PacketHandler { get; }
		NetworkPlayerState CurrentState { get; }

		Task SendPacketAsync(IOutPacket packet);

		Task DisconnectAsync();

		void SetPacketHandler(PacketHandler packetHandler);
	}
}