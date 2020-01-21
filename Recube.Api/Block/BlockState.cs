using System.Collections.Generic;

namespace Recube.Api.Block
{
	public class BlockState
	{
		public readonly bool Default;

		public readonly int Id;

		// PROPERTY NAME, PROPERTY CONDITION
		public readonly Dictionary<string, object>? Properties;

		public BlockState(int id, bool @default, Dictionary<string, object>? properties)
		{
			Id = id;
			Properties = properties;
			Default = @default;
		}

		public static float TotalNumberOfStates { get; set; } = 14;

		public static uint GetGlobalPaletteIdFromState(BlockState state)
		{
			//TODO: Implement
			return 0;
		}

		public static BlockState GetStateFromGlobalPaletteId(uint id)
		{
			//TODO Return new BlockState
			return new BlockState(0, true, new Dictionary<string, object>());
		}

		public bool? ReadPropertyAsBool(string name)
		{
			if (!Properties.ContainsKey(name)) return null;

			return (bool?) Properties.GetValueOrDefault(name, null);
		}

		public int? ReadPropertyAsInt(string name)
		{
			if (!Properties.ContainsKey(name)) return null;

			return (int?) Properties.GetValueOrDefault(name, null);
		}

		public string? ReadPropertyAsString(string name)
		{
			if (!Properties.ContainsKey(name)) return null;

			return (string?) Properties.GetValueOrDefault(name, null);
		}
	}
}