using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Recube.Api.Block;

namespace Recube.Core.Block
{
	class ParsedProperty
	{
		// FIRST: CONDITION LIKE: TRUE
		// SECOND: INT VALUE OF THE ENUM VALUE
		public readonly Dictionary<object, int> Conditions;
		public readonly string PropertyName;

		private ParsedProperty(string propertyName, Dictionary<object, int> conditions)
		{
			PropertyName = propertyName;
			Conditions = conditions;
		}

		public static ParsedProperty Parse(Type t)
		{
			if (!t.IsEnum) throw new PropertyParseException($"Property {t.FullName} is not an Enum");

			var nameAttr = t.GetCustomAttribute<PropertyStateAttribute>(false);
			if (nameAttr == null)
				throw new PropertyParseException(
					$"Property {t.FullName} is missing the {nameof(PropertyStateAttribute)}");


			var values = Enum.GetValues(t);

			var conditions = new Dictionary<object, int>();
			foreach (var value in values)
			{
				var memberInfo = t.GetMember(value.ToString()).First();

				var condition = memberInfo.GetCustomAttribute<PropertyConditionAttribute>(false);
				if (condition == null)
					throw new PropertyParseException(
						$"Field {value.GetType().Name} in property {t.FullName} is missing the {nameof(PropertyConditionAttribute)}");

				conditions.Add(condition.Condition, (int) value);
			}

			return new ParsedProperty(nameAttr.PropertyKey, conditions);
		}
	}
}