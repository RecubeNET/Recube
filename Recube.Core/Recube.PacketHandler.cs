using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Core.Network.Packets;

namespace Recube.Core
{
	public partial class Recube
	{
		public readonly PacketRegistry HandshakeIncomingPacketRegistry = new PacketRegistry();
		public readonly PacketRegistry HandshakeOutgoingPacketRegistry = new PacketRegistry();
		public readonly PacketRegistry LoginIncomingPacketRegistry = new PacketRegistry();
		public readonly PacketRegistry LoginOutgoingPacketRegistry = new PacketRegistry();
		public readonly PacketRegistry PlayIncomingPacketRegistry = new PacketRegistry();
		public readonly PacketRegistry PlayOutgoingPacketRegistry = new PacketRegistry();
		public readonly PacketRegistry StatusIncomingPacketRegistry = new PacketRegistry();
		public readonly PacketRegistry StatusOutgoingPacketRegistry = new PacketRegistry();


		public IPacketRegistry GetCorrectPacketRegistry(NetworkPlayerState state, PacketDirection direction)
		{
			return state switch
			{
				NetworkPlayerState.Handshake => (direction == PacketDirection.Inbound
					? HandshakeIncomingPacketRegistry
					: HandshakeOutgoingPacketRegistry),
				NetworkPlayerState.Play => (direction == PacketDirection.Inbound
					? PlayIncomingPacketRegistry
					: PlayOutgoingPacketRegistry),
				NetworkPlayerState.Status => (direction == PacketDirection.Inbound
					? StatusIncomingPacketRegistry
					: StatusOutgoingPacketRegistry),
				NetworkPlayerState.Login => (direction == PacketDirection.Inbound
					? LoginIncomingPacketRegistry
					: LoginOutgoingPacketRegistry),
				_ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
			};
		}

		private void RegisterPackets()
		{
			var packetClassesTypeList = Assembly.GetExecutingAssembly().GetTypes()
				.Where(t => string.Equals(t.Namespace, "Recube.Core.Network.PacketList", StringComparison.Ordinal))
				.ToImmutableArray();

			var registered = 0;
			foreach (var type in packetClassesTypeList)
			{
				if (!typeof(IPacket).IsAssignableFrom(type)) continue;


				var attr = type.GetCustomAttribute<PacketAttribute>(false);
				if (attr == null) continue;

				if (type.GetConstructor(Type.EmptyTypes) == null)
					throw new InvalidOperationException(
						"Tried to register a IPacket which does not have a parameterless constructor");

				if (typeof(IInPacket).IsAssignableFrom(type))
				{
					RegisterPacket(PacketDirection.Inbound);
				}

				if (typeof(IOutPacket).IsAssignableFrom(type))
				{
					RegisterPacket(PacketDirection.Outbound);
				}

				void RegisterPacket(PacketDirection direction)
				{
					registered++;
					var packetRegistry = GetCorrectPacketRegistry(attr.State, direction);
					packetRegistry.RegisterPacket(attr.Id, (IPacket) Activator.CreateInstance(type));
				}
			}

			RecubeLogger.Info($"Loaded {registered} packets! (IInOutPacket counts twice)");
		}
	}
}