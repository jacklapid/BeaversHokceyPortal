using DataModel;
using DataModel.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LanguageParser
{
    public class LanguageParser : ILanguageParser
    {
        private ILanguageableDataModelContext _languageableContext;



        /*

{{[[Game:Next]].[[DateTime:Date]]}}

{{[[Game:Next]].[[Player:Confirmed]]}}

*/

        public LanguageParser(ILanguageableDataModelContext languageableContext)
        {
            this._languageableContext = languageableContext;
        }

        public string ParseForManager(string inputString, int managerId)
        {
            this._languageableContext.ManagerId = managerId;

            return this.Parse(inputString);
        }

        private string Parse(string inputString)
        {
            var sb = new StringBuilder(inputString);

            var hashedLanguageFragments = new Dictionary<string, string>();

            foreach (var languageFragmentString in GetLanguageFragmentStrings(inputString))
            {
                var languageFragment = LanguageFragment.Parse(languageFragmentString);
                var languageFragmentHash = languageFragment.Hash();

                if (!hashedLanguageFragments.ContainsKey(languageFragmentHash))
                {
                    hashedLanguageFragments.Add(languageFragmentHash, languageFragment.Compute(this._languageableContext));
                }

                sb.Replace("{{" + languageFragmentString + "}}", hashedLanguageFragments[languageFragmentHash]);
            }

            return sb.ToString();
        }

        internal static List<string> GetLanguageFragmentStrings(string inputString)
        {
            var languageFragments = new List<string>();

            var expression = @"(.*?{{(?<fragment>[^\}{2}]+?)}}.*?)*";
            var match = Regex.Match(inputString, expression);

            if (match.Success)
            {
                for (int i = 0; i < match.Groups["fragment"].Captures.Count; i++)
                {
                    languageFragments.Add(match.Groups["fragment"].Captures[i].Value);
                }
            }

            return languageFragments;
        }
    }
}
