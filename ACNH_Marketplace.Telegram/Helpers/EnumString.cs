// <copyright file="EnumString.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Helpers
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Common class for Enum extensions.
    /// </summary>
    public static class EnumString
    {
        /// <summary>
        /// Get Enums entry description from <see cref="DescriptionAttribute"/>.
        /// </summary>
        /// <param name="e">Enum type.</param>
        /// <returns>Enum entry description.</returns>
        public static string GetDescription(this Enum e)
        {
            return GetDescription<DescriptionAttribute>(e);
        }

        /// <summary>
        /// Get Enums entry description from generic attribute type.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <param name="e">Enum type.</param>
        /// <returns>Enum entry description.</returns>
        public static string GetDescription<T>(this Enum e)
            where T : DescriptionAttribute
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

        /// <summary>
        /// Convert string to Enum entry by <see cref="DescriptionAttribute"/>.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="description">Enum entry description.</param>
        /// <returns>Enum entry of generic type.</returns>
        public static T ToDescription<T>(this string description)
            where T : struct, IConvertible
        {
            var type = typeof(T);
            if (type.IsEnum)
            {
                foreach (var field in type.GetFields())
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                    {
                        if (attribute.Description == description)
                        {
                            return (T)field.GetValue(null);
                        }
                    }
                    else
                    {
                        if (field.Name == description)
                        {
                            return (T)field.GetValue(null);
                        }
                    }
                }

                throw new ArgumentException("Not found.", nameof(description));
            }

            throw new NotSupportedException($"{typeof(T)} is not Enum");
        }
    }
}
