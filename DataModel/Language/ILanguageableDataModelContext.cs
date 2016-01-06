using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Language
{
    public interface ILanguageableDataModelContext
    {
        IEnumerable<ILanguagable> GetGames();

        int? ManagerId { get; set; }
    }
}
