using System;
using System.ComponentModel;

namespace ACNH_Marketplace.Telegram.Helpers
{
    public static class EnumString
    {
        public static string GetDescription(this Enum e)
        {
            return GetDescription<DescriptionAttribute>(e);
        }

        public static string GetDescription<T>(this Enum e) where T : DescriptionAttribute
        {
            string description = null;

            if (e is Enum)
            {
                Type type = e.GetType();
                var memInfo = type.GetMember(type.GetEnumName(e));
                var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(T), false);
                foreach (var descriptionAttribute in descriptionAttributes)
                {
                    if (descriptionAttributes.Length > 0 &&
                        typeof(T).ToString() == descriptionAttribute.GetType().ToString())
                    {
                        description = ((T)descriptionAttribute).Description;
                    }
                }
            }

            return description;
        }

        public static T ToDescription<T>(this string description) where T : struct, IConvertible
        {
            var type = typeof(T);
            if (type.IsEnum)
            {
                foreach (var field in type.GetFields())
                {
                    var attribute = Attribute.GetCustomAttribute(field,
                        typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attribute != null)
                    {
                        if (attribute.Description == description)
                            return (T)field.GetValue(null);
                    }
                    else
                    {
                        if (field.Name == description)
                            return (T)field.GetValue(null);
                    }
                }
                throw new ArgumentException("Not found.", nameof(description));
            }
            throw new NotSupportedException($"{typeof(T)} is not Enum");
        }
    }
}
