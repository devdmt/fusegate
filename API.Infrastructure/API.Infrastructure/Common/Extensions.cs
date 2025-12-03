

using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace API.Infrastructure.Common
{
    public static class Extensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            object[] attr = enumValue.GetType().GetField(enumValue.ToString())!
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attr.Length > 0)
                return ((DescriptionAttribute)attr[0]).Description;
            string result = enumValue.ToString();
            result = Regex.Replace(result, "([a-z])([A-Z])", "$1 $2");
            result = Regex.Replace(result, "([A-Za-z])([0-9])", "$1 $2");
            result = Regex.Replace(result, "([0-9])([A-Za-z])", "$1 $2");
            result = Regex.Replace(result, "(?<!^)(?<! )([A-Z][a-z])", " $1");
            return result;
        }

        public static List<string> GetDescriptionList(this Enum enumValue)
        {
            string result = enumValue.GetDescription();
            return result.Split(',').ToList();
        }

        public static string GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }
    }
}
