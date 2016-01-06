﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailModule
{
    interface IEmailSender
    {
        bool SendEmail(string from, string to, string subject, string body);
    }
}
