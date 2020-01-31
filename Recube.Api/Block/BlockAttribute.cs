using System;

namespace Recube.Api.Block
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BlockAttribute : Attribute
    {
        public readonly string Name;

        public BlockAttribute(string name)
        {
            Name = name;
        }
    }
}