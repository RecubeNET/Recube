using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Recube.Api.Entities;

namespace Recube.Core.Entities
{
	public class EntityRegistry : IEntityRegistry
	{
		private readonly ConcurrentDictionary<uint, Entity> _dictionary = new ConcurrentDictionary<uint, Entity>();
		private volatile uint _lastId;

		public uint RegisterEntity(Entity entity)
		{
			var id = GetFreeId();
			_dictionary.TryAdd(id, entity);

			return id;
		}

		public Entity? DeregisterEntity(uint id)
		{
			_dictionary.Remove(id, out var ent);
			return ent;
		}

		public Entity? GetEntityById(uint id)
		{
			return _dictionary.GetValueOrDefault(id);
		}

		private uint GetFreeId()
		{
			var started = _lastId;
			while (true)
			{
				var id = _lastId++;
				if (started == id) throw new InvalidOperationException("All ids occupied");

				var ent = _dictionary.GetValueOrDefault(id);
				if (ent == null)
					return id;
			}
		}
	}
}