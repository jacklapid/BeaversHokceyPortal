using DataModel.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities.CustomAttributes;

namespace LanguageParser
{
    internal class LanguageFragment
    {
        internal static string ENTITY_SEPARATOR = ", ";
        public LanguageFragment()
        {
            this.LanguageLevels = new List<LanguageLevel>();
        }

        public List<LanguageLevel> LanguageLevels { get; set; }

        private string _stringified = null;
        private string Stringified
        {
            get
            {
                if (this._stringified == null)
                {
                    this._stringified = Stringify();
                }

                return this._stringified;
            }
        }
        private string Stringify()
        {
            return
                string.Join(".", LanguageLevels
                .OrderBy(ll => ll.LevelIndex)
                .Select(ll => ll.ToString()));
        }

        public override string ToString()
        {
            return Stringified;
        }

        public string Hash()
        {
            var encodedString = new UTF8Encoding().GetBytes(this.Stringified);
            var hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedString);

            return BitConverter.ToString(hash);
        }

        public string Compute(ILanguageableDataModelContext languageableContext)
        {
            if (!Validate())
            {
                return "{{ Invalid Language Fragment. Please contact the administrator}}";
            }

            var result = string.Empty;

            try
            {
                var enumType = typeof(LanguageConstants.Levels);
                IEnumerable<ILanguagable> languageableEntities = null;

                foreach (var level in this.LanguageLevels.OrderBy(level => level.LevelIndex))
                {
                    var levelItemEnum = (LanguageConstants.Levels)Enum.Parse(enumType, level.ItemName);

                    if (level.ItemName == LanguageConstants.Levels.Self.ToString())
                    {
                        result = string.Join(ENTITY_SEPARATOR, languageableEntities
                            .Select(le => le.GetSelfValue(level.HasAttribute ? level.LanguageAttribute.ItemName : string.Empty)));

                        break;
                    }
                    else
                    {
                        if (level.LevelIndex == 0)
                        {
                            switch (levelItemEnum)
                            {
                                case LanguageConstants.Levels.Game:
                                    languageableEntities = languageableContext.GetGames();
                                    break;
                                default:
                                    throw new ApplicationException("Not supported TOP Level Item: " + level.ItemName);
                            }
                        }
                        else
                        {
                            switch (levelItemEnum)
                            {
                                case LanguageConstants.Levels.Game:
                                    languageableEntities = languageableEntities.SelectMany(le => le.GetGames());
                                    break;
                                case LanguageConstants.Levels.Player:
                                    languageableEntities = languageableEntities.SelectMany(le => le.GetPlayers());
                                    break;
                                default:
                                    throw new ApplicationException("Not supported Level Item: " + level.ItemName);
                            }
                        }

                        if (level.HasAttribute)
                        {
                            languageableEntities = languageableEntities.Where(le => le.FilteredEntity(level.LanguageAttribute.ItemName));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LoggingModule.Logger.Instance.LogError("Error parsing Language Fragment: " + this.ToString(), ex);

                result = "{{ Error in Language Fragment. Please contact the administrator}}";
            }

            return result;
        }

        private bool Validate()
        {
            if (!this.LanguageLevels.Any())
            {
                return true;
            }

            var isValid = true;

            // Validate: Last level must be SELF
            var lastLevel = this.LanguageLevels.OrderBy(level => level.LevelIndex).Last();

            if (lastLevel.ItemName != LanguageConstants.Levels.Self.ToString())
            {
                LoggingModule.Logger.Instance.LogWarning("The last language fragment level must be 'Self'");
                isValid = false;
            }

            // Validate: First level cannot be SELF
            var firstLevel = this.LanguageLevels.OrderBy(level => level.LevelIndex).First();

            if (firstLevel.ItemName == LanguageConstants.Levels.Self.ToString())
            {
                LoggingModule.Logger.Instance.LogWarning("The First language fragment level must not be 'Self'");
                isValid = false;
            }

            // All level attributes must match allow definition: LanguageConstants.LevelAttrubutes
            foreach (var level in this.LanguageLevels)
            {
                if (level.HasAttribute)
                {
                    var definedItemsForAttribute = Utilities.EnumHelper<LanguageConstants.LevelAttrubutes>.GetEnumAttributeValues<ForItem>(level.LanguageAttribute.ToString());

                    var isLevelAttributeValid = !definedItemsForAttribute.Any() || definedItemsForAttribute.Contains(level.ItemName);

                    if (!isLevelAttributeValid)
                    {
                        LoggingModule.Logger.Instance.LogWarning($"The language fragment level {level.ToString()} has attributes ({level.LanguageAttribute.ToString()}) not fitting the item ({level.ItemName}).");
                    }

                    isValid &= isLevelAttributeValid;
                }
            }

            return isValid;
        }

        internal static LanguageFragment Parse(string languageString)
        {
            int level = 0;

            var languageFragment = new LanguageFragment();

            foreach (var languageFragmentLevelString in GetLanguageFragmentLevels(languageString))
            {
                languageFragment.LanguageLevels.Add(LanguageLevel.Parse(languageFragmentLevelString, level));

                level++;
            }

            return languageFragment;
        }

        internal static List<string> GetLanguageFragmentLevels(string languageString)
        {

            //var expression = @"(\[{2}(?<level>\w+(\:|\:\w+)?)\]{2}(\.)?)+";
            var expression = @"(\[\[(?<level>\w+(\:|\:\w+)?)]](\.)?)+";

            var match = Regex.Match(languageString, expression);

            var languageFragmentLevels = new List<string>();

            if (match.Success)
            {
                for (int i = 0; i < match.Groups["level"].Captures.Count; i++)
                {
                    languageFragmentLevels.Add(match.Groups["level"].Captures[i].Value);
                }
            }

            return languageFragmentLevels;
        }
    }
}
