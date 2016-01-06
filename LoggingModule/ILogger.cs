using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingModule
{
    interface ILogger
    {
        void LogInfo(string info);

        void LogError(string error, Exception ex);
    }
}
