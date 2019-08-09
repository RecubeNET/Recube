using System;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets.Handler;

namespace Recube.Core.Network.Packets.Handler
{
	public static class PacketHandlerFactory
	{
		public static PacketHandler CreatePacketHandler(Type type, INetworkPlayer player)
		{
			if (!typeof(PacketHandler).IsAssignableFrom(type))
				throw new InvalidOperationException("Type is not assignable from PacketHandler");
			var packetHandler = (PacketHandler) Activator.CreateInstance(type, player);
			return packetHandler;
		}
	}
}