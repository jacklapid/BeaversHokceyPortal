using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class EnumHelper
    {
        public static Dictionary<int, String> ToDictionary(Type enumType)
        {
            var dic = new Dictionary<int, String>();

            foreach (var enumValue in Enum.GetValues(enumType))
            {
                var name = Enum.GetName(enumType, enumValue);

                dic.Add((int)enumValue, name);
            }

            return dic;
        }
    }
}
