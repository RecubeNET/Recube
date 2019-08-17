using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Recube.Api.Block;

namespace Recube.Core.Block
{
	internal class ParsedBlock
	{
		public readonly string Name;
		public readonly List<ParsedProperty> NeededProperties;

		private ParsedBlock(string name, List<ParsedProperty> neededProperties)
		{
			Name = name;
			NeededProperties = neededProperties;
		}

		public static ParsedBlock Parse(Type t)
		{
			if (!typeof(BaseBlock).IsAssignableFrom(t))
				throw new BlockParseException($"BaseBlock is not assignable from {t.FullName}");

			var blockAttr = t.GetCustomAttribute<BlockAttribute>(false);
			if (blockAttr == null)
				throw new BlockParseException($"{t.FullName} is missing the {nameof(BlockAttribute)}");

			var properties = blockAttr.Properties.Select(ParsedProperty.Parse).ToList();

			return new ParsedBlock(blockAttr.Name, properties);
		}
	}
}