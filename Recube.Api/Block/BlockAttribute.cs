using System;

namespace Recube.Api.Block
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class BlockAttribute : Attribute
	{
		public readonly string Name;
		public readonly Type[] Properties;

		public BlockAttribute(string name, params Type[] properties)
		{
			Name = name;
			Properties = properties;
		}
	}
}