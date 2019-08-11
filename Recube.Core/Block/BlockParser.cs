using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Recube.Api;
using Recube.Api.Block;

namespace Recube.Core.Block
{
	public class BlockParser
	{
		private const string BlockNamespace = "Recube.Api.Block.Impl";
		private readonly string _blocksJsonPath;

		public BlockParser(string blocksJsonPath)
		{
			_blocksJsonPath = blocksJsonPath;
		}

		internal async Task<Dictionary<ParsedBlock, List<BlockState>>> Parse()
		{
			if (!File.Exists(_blocksJsonPath)) throw new FileParseException("Could not find blocks.json");

			// GET ALL CLASSES VIA REFLECTIONS
			var blockClasses = Assembly.GetAssembly(typeof(IRecube)).GetTypes()
				.Where(t => t.Namespace != null && t.Namespace.StartsWith(BlockNamespace))
				.Where(t => t.GetCustomAttribute<NoParseAttribute>(false) == null)
				.Where(t => typeof(BaseBlock).IsAssignableFrom(t))
				.ToImmutableArray();

			// PARSE BLOCK; GET IT'S NAME AND REQUIRED PROPERTIES
			var parsedBlocks = blockClasses.Select(ParsedBlock.Parse).ToList();


			// PARSE FILE
			var fileContent = await File.ReadAllTextAsync(_blocksJsonPath);

			var finishedBlocks = new Dictionary<ParsedBlock, List<BlockState>>();
			try
			{
				var mainJObject = JObject.Parse(fileContent);
				foreach (var parsedBlock in parsedBlocks)
				{
					var blockData = (JObject) mainJObject[parsedBlock.Name];
					if (blockData == null)
						throw new FileParseException(
							$"Could not find block {parsedBlock.Name} in blocks.json! Maybe Recube is outdated?");

					var propertiesData = blockData["properties"] as JObject; // NULLABLE. 
					var shouldHaveProperties = propertiesData != null;

					// CHECK IF REQUIRED PROPERTIES MATCH WITH THESE IN blocks.json
					if (shouldHaveProperties)
					{
						foreach (var parsedProperty in parsedBlock.NeededProperties)
						{
							var propertyData = (JArray) propertiesData[parsedProperty.PropertyName];
							if (propertyData == null)
								throw new FileParseException(
									$"Could not find property {parsedProperty.PropertyName} in blocks.json (Required by {parsedBlock.Name})");

							foreach (var condition in parsedProperty.Conditions)
							{
								foreach (var jToken in propertyData)
								{
									try
									{
										jToken.ToObject(condition.Key.GetType());
									}
									catch (FormatException e)
									{
										throw new FileParseException(
											$"Could not map {jToken.Type} to {condition.Key.GetType().Name} in property \"{parsedProperty.PropertyName}\" in block {parsedBlock.Name}");
									}
								}
							}
						}
					}

					if (!shouldHaveProperties && parsedBlock.NeededProperties.Count > 0)
						throw new FileParseException(
							$"Block {parsedBlock.Name} requires properties, but no properties could be found");

					// PARSE STATES
					var statesData = blockData["states"];
					if (statesData == null)
						throw new FileParseException(
							$"Could not find states array for block {parsedBlock.Name} in blocks.json");

					var states = new List<BlockState>();

					foreach (var jToken in statesData)
					{
						var stateData = (JObject) jToken;
						var idNullable = (int?) stateData["id"];
						if (idNullable == null)
							throw new FileParseException($"{parsedBlock.Name} is missing an id in at least one state");
						var id = idNullable.Value;
						var isDefault = stateData["default"] != null;
						var properties = stateData["properties"] as JObject; // NULLABLE.

						if (properties == null)
						{
							states.Add(new BlockState(id, isDefault, null));
							continue;
						}

						var props = new Dictionary<string, object>();

						foreach (var kvp in properties)
						{
							var parsedProperty =
								parsedBlock.NeededProperties.FirstOrDefault(p => p.PropertyName == kvp.Key);

							var parsedFirstCondition = parsedProperty?.Conditions.First(kv => true).Key;
							if (parsedFirstCondition == null)
								throw new FileParseException(
									$"Could not find property {kvp.Key} for block {parsedBlock.Name}");

							if (!props.TryAdd(kvp.Key, kvp.Value.ToObject(parsedFirstCondition.GetType())))
								throw new FileParseException(
									$"Property {kvp.Key} exists multiple times for block {parsedBlock.Name}");
						}

						states.Add(new BlockState(id, isDefault, props));
					}

					finishedBlocks.Add(parsedBlock, states);
				}
			}
			catch (JsonReaderException e)
			{
				throw new FileParseException($"Could not parse blocks.json", e);
			}

			return finishedBlocks;
		}
	}
}