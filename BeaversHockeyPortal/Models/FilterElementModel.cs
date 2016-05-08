using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace BeaversHockeyPortal.Models
{
    public class FilterElementModel
    {
        public IEnumerable<SelectListItem> Items { get; set; }

        public string SelectedValue { get; set; }

        public string ModelElementName { get; set; }
    }
}