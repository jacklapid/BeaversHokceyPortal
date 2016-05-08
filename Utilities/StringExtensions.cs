using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class StringExtensions
    {
        public static bool TryConvert<T>(this string input, out T result)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));

            if (converter != null)
            {
                try
                {
                    result = (T)converter.ConvertFromString(input);

                    return true;
                }
                catch
                {
                    result = default(T);
                    return false;
                }
            }

            result = default(T);
            return false;
        }
    }
}
