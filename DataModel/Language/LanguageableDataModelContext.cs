using DataModel.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class DataModelContext : ILanguageableDataModelContext
    {
        public int? ManagerId { get; set; }

        public IEnumerable<ILanguagable> GetGames()
        {
            List<Game> games = null;
            if (this.ManagerId.HasValue)
            {
                games = this.Games.Where(g => g.Manager.Id == this.ManagerId.Value).ToList();
            }
            else
            {
                games = this.Games.ToList();
            }

            games.ForEach(game => game.SetContext(this));

            return games;
        }
    }
}
