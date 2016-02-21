using DataModel.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class Game : LanguageableEntity
    {
        public override string EntityName
        {
            get
            {
                return "Game";
            }
        }

        public override IEnumerable<ILanguagable> GetPlayers()
        {
            var players = this.ManagerId.HasValue
                ? this.Context.Persons.OfType<Player>().Where(p => p.Manager.Id == this.ManagerId.Value).ToList()
                : this.Context.Persons.OfType<Player>().ToList();

            players.ForEach(p =>
            {
                p.ParentLevelEntityId = this.Id;
                p.SetContext(this.Context);
            });

            return players;
        }

        public override string GetSelfValue(string attribute)
        {
            switch (attribute)
            {
                case "Date":
                    return this.Date.ToLongDateString();
                case "Time":
                    return this.Date.ToShortTimeString();
                case "Full":
                case "":
                    return this.Date.ToString();
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

            var orderedGames = this.Context.Games.OrderBy(g => g.Date);

            switch (attribute)
            {
                case "Next":
                    var nextGame = orderedGames.FirstOrDefault(g => g.Date > DateTime.Now);
                    return nextGame != null && nextGame.Id == this.Id;
                case "Previous":
                    var previousGame = orderedGames.LastOrDefault(g => g.Date < DateTime.Now);
                    return previousGame != null && previousGame.Id == this.Id;
                case "AfterNext":
                    var nextTwoGames = orderedGames.Where(g => g.Date > DateTime.Now).Take(2);
                    return nextTwoGames.Any(nextG => nextG.Id == this.Id);
                default:
                    throw new ApplicationException("Not supported Level filtering attribute for entity Game. Attribute: " + attribute);
            }
        }
    }
}
