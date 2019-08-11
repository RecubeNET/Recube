using System;

namespace Recube.Api.Block
{
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