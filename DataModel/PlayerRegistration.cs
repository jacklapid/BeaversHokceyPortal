using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class PlayerRegistration
    {
        public int Id { get; set; }

        public string  PlayerEmail { get; set; }

        public string  Token { get; set; }

        public DateTime TokenGeneratedOn { get; set; }

        public bool TokenAlreadyUsed { get; set; }

        public Manager Manager { get; set; }

        public Team Team { get; set; }

    }
}
