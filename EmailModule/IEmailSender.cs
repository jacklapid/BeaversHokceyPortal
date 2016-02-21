using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailModule
{
    public interface IEmailSender
    {
        bool SendEmail(string from, string to, string subject, string body);
    }
}
