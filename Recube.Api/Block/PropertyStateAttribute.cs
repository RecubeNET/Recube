using System;

namespace Recube.Api.Block
{
    /// <summary>
    /// This will be put on the enum values in combination with <see cref="PropertyConditionAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class PropertyStateAttribute : Attribute
    {
        public readonly string PropertyKey;

        public PropertyStateAttribute(string propertyKey)
        {
            PropertyKey = propertyKey;
        }
    }
}