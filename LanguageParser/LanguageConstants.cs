using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.CustomAttributes;

namespace LanguageParser
{
    public class LanguageConstants
    {
        public enum Levels
        {
            [Description("Information about a Game")]
            Game,

            [Description("Player for the event)")]
            Player,

            [Description("Final node in the language chain, describing the end entity")]
            Self,
        }

        public enum LevelAttrubutes
        {
            [Description("Next Game")]
            [ForItem("Game")]
            Next,

            [Description("Previous Game")]
            [ForItem("Game")]
            Previous,

            [Description("The game After the Next Game")]
            [ForItem("Game")]
            AfterNext,

            [Description("The DATE part of the date-time")]
            [ForItem("Self")]
            Date,

            [Description("The TIME part of the date-time")]
            [ForItem("Self")]
            Time,

            [Description("The DATE AND TIME part of the date-time")]
            [ForItem("Self")]
            Full,

            [Description("The name for the entity")]
            [ForItem("Self")]
            Name,

            [ForItem("Player")]
            Regular,
            [ForItem("Player")]
            Spare,
            [ForItem("Player")]
            Goalie,
            [ForItem("Player")]
            Skater,

            [Description("All Confirmed players")]
            [ForItem("Player")]
            Confirmed,

            [Description("Confirmed Regulars")]
            [ForItem("Player")]
            ConfirmedRegular,

            [Description("Confirmed Spares")]
            [ForItem("Player")]
            ConfirmedSpare,

            [Description("UN-Confirmed Regulars")]
            [ForItem("Player")]
            UnConfirmedRegular,

            [Description("UN-Confirmed Spares")]
            [ForItem("Player")]
            UnConfirmedSpare,
        }

        public static string GetLanguageHelp()
        {
            return "TODO: LANGUEGE HELP";
        }
    }
}
