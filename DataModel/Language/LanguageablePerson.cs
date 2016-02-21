using DataModel.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class Person : LanguageableEntity
    {
        public override string EntityName
        {
            get
            {
                return "Person";
            }
        }

        public override string GetSelfValue(string attribute)
        {
            switch (attribute)
            {
                case "Email":
                    return this.Context.Users.First(u => u.Id == this.ApplicationUser_Id).Email;
                case "Name":
                case "":
                    return this.FullName;
                default:
                    throw new ApplicationException("Not supported SELF attribute for entity Game. Attribute: " + attribute);
            }
        }
        public override bool FilteredEntity(string attribute)
        {
            if (string.IsNullOrWhiteSpace(attribute))
            {
                return true;
            }

            var player = this as Player;

            if (player == null)
            {
                return false;
            }

            switch (attribute)
            {
                case "Regular":
                    return player.IsRegular;
                case "Spare":
                    return player.IsSpare;
                case "Goalie":
                    return player.IsGoalie;
                case "Skater":
                    return player.IsPlayer;
                case "Confirmed":
                    return this.IsConfirmedForGame();
                case "ConfirmedRegular":
                    return this.IsConfirmedForGame() && player.IsRegular;
                case "UnConfirmedRegular":
                    return !this.IsConfirmedForGame() && player.IsRegular;
                case "UnConfirmedSpare":
                    return !this.IsConfirmedForGame() && player.IsSpare;
                default:
                    throw new ApplicationException("Not supported Level filtering attribute for entity Player. Attribute: " + attribute);
            }
        }


        private bool IsConfirmedForGame()
        {
            var gameExists = this.ParentLevelEntityId.HasValue && this.Context.Games.Any(g => g.Id == this.ParentLevelEntityId.Value);

            if (!gameExists)
            {
                throw new ApplicationException($"While Verifying confirmation for player: {this.FullName}, the game against which to verify was not supplied!");
            }

            return this.Context.GameConfirmations.Any(gc => gc.Game_Id == this.ParentLevelEntityId.Value && gc.Player_Id == this.Id);
        }
    }
}
