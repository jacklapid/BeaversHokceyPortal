using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageParser
{
    internal class LanguageLevel
    {
        public static char[] ATTRIBUTE_DELIMITER = new [] { ':' };

        public int LevelIndex { get; set; }

        public string ItemName { get; set; }

        public LanguageAttribute LanguageAttribute { get; set; }

        public bool HasAttribute
        {
            get { return LanguageAttribute != null; }
        }

        public override string ToString()
        {
            var languageAttrubuteString = string.Empty;
            if (this.LanguageAttribute != null)
            {
                languageAttrubuteString = $":{LanguageAttribute.ToString()}";
            }

            return $"[[{ItemName}{languageAttrubuteString}]]";
        }

        public static LanguageLevel Parse(string languageFragmentLevelString, int level)
        {
            var itemAndAttribute = languageFragmentLevelString.Split(ATTRIBUTE_DELIMITER, StringSplitOptions.RemoveEmptyEntries);

            var languageLevel = new LanguageLevel
            {
                LevelIndex = level,
                ItemName = itemAndAttribute[0]
            };

            if (itemAndAttribute.Count() > 1)
            {
                languageLevel.LanguageAttribute = new LanguageAttribute
                {
                    ItemName = itemAndAttribute[1]
                };
            }

            return languageLevel;
        }
    }
}
