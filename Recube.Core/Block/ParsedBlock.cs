using System;
using System.Collections.Generic;
using System.Reflection;

namespace Recube.Core.Block
{
    public class ParsedBlock
    {
        public string Name { get; }
        public Type BaseBlockType { get; }
        public List<ParsedProperty> Properties { get; }

        public ConstructorInfo ConstructorInfo { get; }

        public ParsedBlock(string name, Type baseBlockType, List<ParsedProperty> properties,
            ConstructorInfo constructorInfo)
        {
            Name = name;
            BaseBlockType = baseBlockType;
            Properties = properties;
            ConstructorInfo = constructorInfo;
        }
    }
}