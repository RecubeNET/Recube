using System;

namespace Recube.Api.Block
{
    /// <summary>
    /// This attribute will be used in Enum fields to declare what condition the property needs to meet to become this specific enum value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PropertyConditionAttribute : Attribute
    {
        public readonly string Condition;

        public PropertyConditionAttribute(string condition)
        {
            Condition = condition;
        }
    }
}