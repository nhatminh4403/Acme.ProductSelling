using System;
using System.ComponentModel;
using System.Reflection;

namespace Acme.ProductSelling.Web.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumDescriptions(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo == null)
            {
                return value.ToString();
            }

            DescriptionAttribute attr = fieldInfo.GetCustomAttribute<DescriptionAttribute>();

            return attr == null ? value.ToString() : attr.Description;
        }
    }
}
