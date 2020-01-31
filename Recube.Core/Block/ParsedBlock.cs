using System;
using System.Collections.Generic;

namespace Recube.Core.Block
{
    public class ParsedBlock
    {
        public string Name { get; }
        public Type BaseBlock { get; }
        public List<ParsedProperty> Properties { get; }

        public ParsedBlock(string name, Type baseBlock, List<ParsedProperty> properties)
        {
            Name = name;
            BaseBlock = baseBlock;
            Properties = properties;
        }
    }
}