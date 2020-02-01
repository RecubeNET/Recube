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
    public static class BlockParser
    {
        public static async Task<Dictionary<string, List<BlockState>>> ParseFile(string blocksJsonPath)
        {
            var ret = new Dictionary<string, List<BlockState>>();
            var file = await File.ReadAllTextAsync(blocksJsonPath);
            try
            {
                var mainJObject = JObject.Parse(file);
                foreach (var block in mainJObject)
                {
                    var name = block.Key;

                    if (!(block.Value is JObject blockBody))
                        throw new FileParseException($"Invalid blocks.json. Block {name} has no body");

                    Dictionary<string, List<JToken>>? properties = null;
                    if (blockBody["properties"] is JObject props)
                    {
                        properties = new Dictionary<string, List<JToken>>();
                        foreach (var prop in props)
                        {
                            var propName = prop.Key;
                            if (!(prop.Value is JArray propValue))
                                throw new FileParseException(
                                    $"Invalid blocks.json. Block's ({name}) property {propName} is missing a valid body");
                            properties.Add(propName, propValue.ToList());
                        }
                    }


                    var blockStates = new List<BlockState>();

                    if (!(blockBody["states"] is JArray states))
                        throw new FileParseException($"Invalid blocks.json. Block {name} is missing states array");

                    foreach (var state in states)
                    {
                        var id = (int?) state["id"];
                        if (id == null)
                            throw new FileParseException($"Invalid blocks.json. A block's ({name}) state has no id");
                        var @default = ((bool?) state["default"]) ?? false;


                        var blockStateProps = new Dictionary<string, string>();
                        if (properties != null)
                        {
                            if (!(state["properties"] is JObject stateProps))
                                throw new FileParseException(
                                    $"Invalid blocks.json. The state {id.Value} of block {name} has no properties");
                            foreach (var stateProp in stateProps)
                            {
                                blockStateProps.Add(stateProp.Key, stateProp.Value.ToObject<string>());
                            }
                        }

                        blockStates.Add(new BlockState(name, id.Value, @default,
                            blockStateProps.Count == 0 ? null : blockStateProps));
                    }

                    ret.Add(name, blockStates);
                }
            }
            catch (JsonReaderException e)
            {
                throw new FileParseException("Could not parse blocks.json", e);
            }

            return ret;
        }

        public static List<ParsedBlock> ParseBlockClasses(string blockNamespace)
        {
            var blockClasses = Assembly.GetAssembly(typeof(IRecube)).GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.StartsWith(blockNamespace))
                .Where(t => t.GetCustomAttribute<NoParseAttribute>(false) == null)
                .Where(t => typeof(BaseBlock).IsAssignableFrom(t))
                .ToImmutableArray();

            var ret = new List<ParsedBlock>();


            foreach (var blockClass in blockClasses)
            {
                var name = blockClass.GetCustomAttribute<BlockAttribute>().Name;

                var properties = new List<ParsedProperty>();

                foreach (var nestedType in blockClass.GetNestedTypes())
                {
                    var propAttr = nestedType.GetCustomAttribute<PropertyStateAttribute>();
                    if (propAttr == null) continue;
                    properties.Add(ParsedProperty.Parse(nestedType));
                }

                var constructor = blockClass.GetConstructor(properties.Select(p => p.Type).ToArray());
                if (constructor == null)
                    throw new BlockParseException(
                        $"Could not find constructor which is buildable with all properties for block {name}");

                ret.Add(new ParsedBlock(name, blockClass, properties, constructor));
            }

            return ret;
        }
    }
}