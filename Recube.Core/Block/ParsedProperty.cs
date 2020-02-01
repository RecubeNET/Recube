using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Recube.Api.Block;

namespace Recube.Core.Block
{
    public class ParsedProperty
    {
        // FIRST: INT VALUE OF THE ENUM VALUE
        // SECOND: CONDITION LIKE: TRUE
        public readonly Dictionary<int, string> Conditions;
        public readonly string PropertyName;
        public readonly FieldInfo Field;
        public readonly Type Type;

        private ParsedProperty(string propertyName, Dictionary<int, string> conditions, FieldInfo field, Type type)
        {
            PropertyName = propertyName;
            Conditions = conditions;
            Field = field;
            Type = type;
        }

        public static ParsedProperty Parse(Type t)
        {
            if (!t.IsEnum) throw new PropertyParseException($"Property {t.FullName} is not an Enum");

            var nameAttr = t.GetCustomAttribute<PropertyStateAttribute>(false);
            if (nameAttr == null)
                throw new PropertyParseException(
                    $"Property {t.FullName} is missing the {nameof(PropertyStateAttribute)}");


            var values = Enum.GetValues(t);

            var conditions = new Dictionary<int, string>();
            foreach (var value in values)
            {
                var memberInfo = t.GetMember(value.ToString()).First();

                var condition = memberInfo.GetCustomAttribute<PropertyConditionAttribute>(false);
                if (condition == null)
                    throw new PropertyParseException(
                        $"Field {value.GetType().Name} in property {t.FullName} is missing the {nameof(PropertyConditionAttribute)}");

                conditions.Add((int) value, condition.Condition);
            }

            var declaringType = t.DeclaringType;
            if (declaringType == null) throw new PropertyParseException($"Property {t.FullName} has no declaring type");

            var field = declaringType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.FieldType == t).ToList();
            if (field.Count > 1)
                throw new PropertyParseException(
                    $"Property {t.FullName}'s declaring type has more than 0 field containing this property");
            if (field.Count == 0)
                throw new PropertyParseException(
                    $"Property {t.FullName}'s declaring type has no fields containing this property");

            return new ParsedProperty(nameAttr.PropertyKey, conditions, field[0], t);
        }
    }
}