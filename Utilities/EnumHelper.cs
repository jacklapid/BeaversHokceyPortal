﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.CustomAttributes;

namespace Utilities
{
    public static class EnumHelper<T>
    {
        /// <summary>
        /// Convert enum to dictionary of Key:Id, Value: Name
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, String> ToDictionary()
        {
            var type = typeof(T);

            var dic = new Dictionary<int, String>();

            foreach (var enumValue in Enum.GetValues(type))
            {
                var name = Enum.GetName(type, enumValue);

                var field = type.GetField(name);
                var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                var description = customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : name;

                dic.Add((int)enumValue, description);
            }

            return dic;
        }

        public static List<string> GetEnumAttributeValues<CustomAttributeT>(string itemName) where CustomAttributeT : CustomAttribute
        {
            var type = typeof(T);

            //var itemName = Enum.GetName(type, enumItem);

            var field = type.GetField(itemName);
            var customAttribute = field.GetCustomAttributes(typeof(CustomAttributeT), false);

            var attributeValues = customAttribute.Length > 0 ? ((CustomAttribute)customAttribute[0]).AttributeValues : new List<string>();
            return attributeValues;
        }

    }
}
