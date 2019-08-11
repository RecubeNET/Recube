using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Recube.Api.Block;

namespace Recube.Core.Block
{
	public class BlockStateRegistry : IBlockStateRegistry
	{
		private readonly Dictionary<string, List<BlockState>> _dictionary = new Dictionary<string, List<BlockState>>();
		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		public bool Register(string blockName, List<BlockState> blockStates)
		{
			_lock.EnterWriteLock();
			try
			{
				return _dictionary.TryAdd(blockName, blockStates);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public void Register(string blockName, params BlockState[] blockStates) =>
			Register(blockName, blockStates.ToList());

		public bool Deregister(string blockName)
		{
			_lock.EnterWriteLock();
			try
			{
				return _dictionary.Remove(blockName);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public BlockState? GetStateByProperty(Type blockClass, Type property, object value)
		{
			if (!typeof(BaseBlock).IsAssignableFrom(blockClass))
				throw new InvalidOperationException($"BaseBlock needs to be assignable from {blockClass.FullName}");
			var blockName = blockClass.GetCustomAttribute<BlockAttribute>(false);
			if (blockName == null)
				throw new InvalidOperationException($"{blockClass.FullName} is missing the {nameof(BlockAttribute)}");
			if (!property.IsEnum)
				throw new InvalidOperationException("Property needs to be an Enum");
			var attr = property.GetCustomAttribute<PropertyStateAttribute>(false);
			if (attr == null)
				throw new InvalidOperationException($"Property needs to have the {nameof(PropertyStateAttribute)}");
			var propertyName = attr.PropertyKey;
			return GetStateByProperty(blockName.Name, propertyName, value);
		}

		public BlockState? GetStateByProperty(string blockName, string propertyName, object value)
		{
			_lock.EnterReadLock();
			try
			{
				var states = _dictionary[blockName];
				if (states == null) return null;
				foreach (var blockState in states)
				{
					var blockStateProperty = blockState.Properties[propertyName];
					if (blockStateProperty == null) return null;
					if (blockStateProperty == value)
					{
						return blockState;
					}
				}
			}
			finally
			{
				_lock.ExitReadLock();
			}

			return null;
		}

		public Dictionary<string, List<BlockState>> GetAll()
		{
			_lock.EnterReadLock();
			try
			{
				return new Dictionary<string, List<BlockState>>(_dictionary);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}
	}
}