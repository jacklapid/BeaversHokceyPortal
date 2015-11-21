using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataModel
{
    public class Manager
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Player> Players { get; set; }
    }
}
