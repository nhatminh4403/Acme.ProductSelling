using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public static List<SelectListItem> ToLocalizedSelectList<TEnum>(this IStringLocalizer localizer)
            where TEnum : Enum
        {
            var enumType = typeof(TEnum);

            return Enum.GetValues(enumType)
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    // The 'Value' will be the enum member name (e.g., "Hdd35Inch")
                    Value = e.ToString(),

                    // The 'Text' will be looked up from your en.json/vi.json file
                    // using the key pattern "Enum:EnumTypeName:EnumMemberName"
                    Text = localizer[$"Enum:{enumType.Name}:{e}"]
                })
                .ToList();
        }
    }
}
