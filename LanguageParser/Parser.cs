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

        public LanguageParser(ILanguageableDataModelContext languageableContext)
        {
            this._languageableContext = languageableContext;
        }

        public List<string> ParseForManager(string inputString, string emailTemplateContext, int managerId, bool aggrigateParsedResult)
        {
            this._languageableContext.ManagerId = managerId;

            return this.Parse(inputString, emailTemplateContext, aggrigateParsedResult);
        }

        private List<string> Parse(string inputString, string emailTemplateContext, bool aggrigateParsedResult)
        {
            if (!HasLanguageFragment(inputString))
            {
                return new List<string>(1) { inputString };
            }

            List<string> results = null;

            foreach (var languageFragmentString in GetLanguageFragmentStrings(inputString))
            {
                // pre-pend language string with language context if any
                var fullLanguageString = !string.IsNullOrWhiteSpace(emailTemplateContext)? $"{emailTemplateContext}.{languageFragmentString}" : languageFragmentString;
                var languageFragment = LanguageFragment.Parse(fullLanguageString);

                // Compute language fragment
                var computedFragments = languageFragment.Compute(this._languageableContext);

                // if the result of parsing is to be aggrigated, we need to String.Join
                if (aggrigateParsedResult)
                {
                    computedFragments = new List<string>
                    {
                        string.Join(", ", computedFragments)
                    };
                }

                // CLONE the input for each occurrence of the ouput (Ex: each copy for each player who would be receiving this email)
                if (results == null)
                {
                    // should the computed string produce NO result, we only clone the input string ONCE
                    var clonesCount = computedFragments.Count() > 0 ? computedFragments.Count() : 1;

                    results = Enumerable.Range(0, computedFragments.Count()).Select(x => inputString).ToList();
                }

                /* We might need to RE-CLONE the string, in case:
                    - The first computed fragment produces ONE result (such as GAME DATE), but the following fragment (Players Names) produced multiples
                 */
                if (results.Count() == 1 && computedFragments.Count() > 1)
                {
                    results = Enumerable.Range(0, computedFragments.Count()).Select(x => results.First()).ToList();
                }

                // Error-scenario, when number computed results does not match the number of result strings 
                // Ex: multiplier 1 : Player Name; multiplier 2: Games in season. We cannot produce this!!!!
                if (computedFragments.Count() != 1 && results.Count() != computedFragments.Count())
                {
                    LoggingModule.Logger.Instance.LogWarning($"number computed results does not match the number of result strings. \nInput string: {inputString}, context: {emailTemplateContext} ");
                }
                else
                {
                    // When all good, replace each fragment is each input string
                    for (int i = 0; i < results.Count(); i++)
                    {
                        results[i] = results[i].Replace("{{" + languageFragmentString + "}}", computedFragments.Count() == 1 ? computedFragments[0] : computedFragments[i]);
                    }
                }
            }

            return results;


            //var sb = new StringBuilder(inputString);

            //var hashedLanguageFragments = new Dictionary<string, string>();

            //foreach (var languageFragmentString in GetLanguageFragmentStrings(inputString))
            //{
            //    var languageFragment = LanguageFragment.Parse(languageFragmentString);
            //    var languageFragmentHash = languageFragment.Hash();

            //    if (!hashedLanguageFragments.ContainsKey(languageFragmentHash))
            //    {
            //        hashedLanguageFragments.Add(languageFragmentHash, languageFragment.Compute(this._languageableContext));
            //    }

            //    sb.Replace("{{" + languageFragmentString + "}}", hashedLanguageFragments[languageFragmentHash]);
            //}

            //return sb.ToString();
        }

        public static bool HasLanguageFragment(string inputString)
        {
            return GetLanguageFragmentStrings(inputString).Any();
        }

        internal static List<string> GetLanguageFragmentStrings(string inputString)
        {
            var languageFragments = new List<string>();

            var expression = @"(.*?{{(?<fragment>[^\}{2}]+?)}}.*?)*";
            var match = Regex.Match(inputString, expression, RegexOptions.Singleline);

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
