using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageParser
{
   internal class LanguageAttribute
    {
        public string ItemName { get; set; }

        public override string ToString()
        {
            return this.ItemName;
        }
    }
}
