using System;

namespace Recube.Api.Block
{
	[AttributeUsage(AttributeTargets.Field)]
	public class PropertyConditionAttribute : Attribute
	{
		public readonly object Condition;

		public PropertyConditionAttribute(object condition)
		{
			Condition = condition;
		}
	}
}