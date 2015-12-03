using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class LinqExtensions
    {
        public static bool IsAny<T>(this IEnumerable<T> items)
        {
            if (items != null)
            {
                return items.Any();
            }

            return false;
        }
    }
}
