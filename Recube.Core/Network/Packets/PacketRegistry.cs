using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Packets
{
	public class PacketRegistry : IPacketRegistry
	{
		private ConcurrentDictionary<int, Type> _packets = new ConcurrentDictionary<int, Type>();

		public bool RegisterPacket<T>(int id, T packet) where T : IPacket
		{
			return _packets.TryAdd(id, packet.GetType());
		}

		public bool DeregisterPacket(int id)
		{
			return _packets.TryRemove(id, out _);
		}

		public IPacket? GetPacketById(int id)
		{
			if (_packets.TryGetValue(id, out var packetType))
			{
				return (IPacket) Activator.CreateInstance(packetType);
			}

			return null;
		}

		public int? GetPacketId(IPacket packet)
		{
			var kvp = _packets.FirstOrDefault(pair => pair.Value == packet.GetType());
			if (kvp.Equals(default(KeyValuePair<int, Type>))) return null;

			return kvp.Key;
		}
	}
}