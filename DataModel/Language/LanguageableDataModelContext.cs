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
            if (this.ManagerId.HasValue)
            {
                return this.Games.Where(g => g.Manager.Id == this.ManagerId.Value).ToList();
            }
            else
            {
                return this.Games.ToList();
            }
        }
    }
}
