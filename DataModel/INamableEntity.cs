using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public interface INamableEntity
    {
        int Id { get; set; }

        string Name { get; }
    }
}
