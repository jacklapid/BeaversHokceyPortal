using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BeaversHockeyPortal.WebUtilities
{
    public static class SelectListItemExtensions
    {
        public static List<SelectListItem> ToSelectListItems<T>(this IQueryable<T> list)
        {
            return list.ToList().ToSelectListItems();
        }

        public static List<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> list)
        {
            return list.ToList().ToSelectListItems();
        }

        public static List<SelectListItem> ToSelectListItems(this Dictionary<int, string> dic)
        {
            return dic.Select(kvp => new SelectListItem
            {
                Text = kvp.Value,
                Value = kvp.Key.ToString()
            })
            .OrderBy(x => x.Text)
            .ToList();
        }

        public static List<SelectListItem> ToSelectListItems<KeyT>(this Dictionary<KeyT, string> dic)
        {
            return dic.Select(kvp => new SelectListItem
            {
                Text = kvp.Value.ToString(),
                Value = kvp.Key.ToString()
            })
            .OrderBy(x => x.Text)
            .ToList();
        }


        private static List<SelectListItem> ToSelectListItems<T>(this List<T> list)
        {
            var selectedListItems = new List<SelectListItem>(list.Count());

            foreach (dynamic item in list)
            {
                selectedListItems.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return selectedListItems.OrderBy(x => x.Text).ToList();
        }
    }
}
