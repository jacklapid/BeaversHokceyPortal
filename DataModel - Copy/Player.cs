using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataModel
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Player
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int PlayerStatusId { get; set; }

        public string Email { get; set; }

        public string ImageUrl { get; set; }

        public int ManagerId { get; set; }
    }
}
