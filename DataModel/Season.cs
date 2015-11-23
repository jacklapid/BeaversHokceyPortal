using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Season
    {
        public int Id { get; set; }

        public DateTime  StartDate{ get; set; }

        public DateTime EndDate { get; set; }

        public string Name { get; set; }
    }
}
