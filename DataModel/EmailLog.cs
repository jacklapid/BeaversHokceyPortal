using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class EmailLog
    {
        public int Id { get; set; }

        public Game Game { get; set; }

        public GameEmailEvent EmailEvent { get; set; }

        public DateTime TimeStamp { get; set; }

        public bool Successful { get; set; }
    }
}
