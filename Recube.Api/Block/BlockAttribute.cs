using System;

namespace Recube.Api.Block
{
    /// <summary>
    /// Will be put on each block class with the block name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BlockAttribute : Attribute
    {
        /// <summary>
        /// The block name. For example "minecraft:grass_block"
        /// </summary>
        public readonly string Name;

        public BlockAttribute(string name)
        {
            Name = name;
        }
    }
}