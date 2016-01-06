using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.CustomAttributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class ForItem : CustomAttribute
    {
        public ForItem(): base()
        {

        }

        public ForItem(params string[] itemNames) : base(itemNames)
        {

        }
    }
}
