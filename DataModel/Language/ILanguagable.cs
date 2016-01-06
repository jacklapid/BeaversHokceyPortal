using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Language
{
    public interface ILanguagable
    {
        string GetSelfValue(string attribute);

        IEnumerable<ILanguagable> GetGames();

        IEnumerable<ILanguagable> GetPlayers();

        bool FilteredEntity(string attribute);

        string EntityName { get; }

        int? ParentLevelEntityId { get; set; }

        int? ManagerId { get; set; }
    }
}
