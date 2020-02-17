using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                        int id;
                        try
                        {
                            id = (int) state["id"];
                        }
                        catch (Exception)
                        {
                            throw new FileParseException($"Invalid blocks.json. A block's ({name}) state has no id");
                        }

                        var @default = ((bool?) state["default"]) ?? false;


                        var blockStateProps = new Dictionary<string, string>();
                        if (properties != null)
                        {
                            if (!(state["properties"] is JObject stateProps))
                                throw new FileParseException(
                                    $"Invalid blocks.json. The state {id} of block {name} has no properties");
                            foreach (var stateProp in stateProps)
                            {
                                blockStateProps.Add(stateProp.Key, stateProp.Value.ToObject<string>());
                            }
                        }

                        blockStates.Add(new BlockState(name, id, @default,
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
            var blockClasses = Assembly.GetCallingAssembly().GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.StartsWith(blockNamespace))
                .Where(t => t.GetCustomAttribute<NoParseAttribute>(false) == null)
                .Where(t => t.GetCustomAttribute<BlockAttribute>() != null)
                .Where(t => typeof(BaseBlock).IsAssignableFrom(t))
                .ToImmutableArray();

            var ret = new List<ParsedBlock>();


            foreach (var blockClass in blockClasses)
            {
                var name = blockClass.GetCustomAttribute<BlockAttribute>().Name;

                var properties = new List<ParsedProperty>();

                var parentsPlusBlockClass = new List<Type>();
                var iteration = 0;
                var baseType = blockClass;
                do
                {
                    parentsPlusBlockClass.Add(baseType);
                    baseType = baseType.BaseType;
                    iteration++;
                } while (baseType != null && iteration < 10 && baseType != typeof(BaseBlock));

                if (iteration == 10)
                    throw new BlockParseException(
                        $"The parent which implements BlockBase is more than 10 inheritors away for block {name}");

                parentsPlusBlockClass.Reverse();
                foreach (var type in parentsPlusBlockClass)
                {
                    foreach (var nestedType in type.GetNestedTypes())
                    {
                        var propAttr = nestedType.GetCustomAttribute<PropertyStateAttribute>();
                        if (propAttr == null) continue;
                        properties.Add(ParsedProperty.Parse(blockClass, nestedType));
                    }
                }


                var constructor = blockClass.GetConstructor(properties.Select(p => p.Type).ToArray());
                if (constructor == null)
                    // When issuing this exception check the order of parameters as defined in the comment in BaseBlock.cs
                    throw new BlockParseException(
                        $"Could not find constructor which is buildable with all properties for block {name}. Is the order correct?");

                ret.Add(new ParsedBlock(name, blockClass, properties, constructor));
            }

            return ret;
        }
    }
}