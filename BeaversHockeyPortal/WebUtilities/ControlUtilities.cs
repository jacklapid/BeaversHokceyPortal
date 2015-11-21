using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BeaversHockeyPortal.WebUtilities
{
    public static class ControlUtilities
    {
        public static IEnumerable<SelectListItem> GetSelectListItem(Type enumType)
        {
            return Enum.GetValues(enumType)
                .Cast<object>()
                .Select(x => new SelectListItem
                {
                    Text = Enum.GetName(enumType, x),
                    Value = x.ToString()
                });
        }

    }
}
