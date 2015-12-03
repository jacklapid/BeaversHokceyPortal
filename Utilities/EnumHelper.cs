using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class EnumHelper<T>
    {
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
    }
}
