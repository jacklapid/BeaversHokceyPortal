using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageParser
{
    interface ILanguageParser
    {
        string ParseForManager(string inputString, int managerId);
    }
}
