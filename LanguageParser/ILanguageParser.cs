﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageParser
{
    public interface ILanguageParser
    {
        List<string> ParseForManager(string inputString, string emailTemplateContext, int managerId, bool aggrigateParsedResult);
    }
}
