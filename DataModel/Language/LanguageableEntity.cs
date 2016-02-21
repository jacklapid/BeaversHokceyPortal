using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Language
{
    public abstract class LanguageableEntity : ILanguagable
    {
        [NotMapped]
        public DataModelContext Context { get; private set; }

        public void SetContext(DataModelContext context)
        {
            this.Context = context;
        }

        public virtual IEnumerable<ILanguagable> GetGames()
        {
            throw new LanguageItemNotSupportedException("Game", this.EntityName);
        }

        public virtual IEnumerable<ILanguagable> GetPlayers()
        {
            throw new LanguageItemNotSupportedException("Player", this.EntityName);
        }

        public virtual string GetSelfValue(string attribute)
        {
            throw new LanguageAttributeNotSupportedException(attribute, this.EntityName);
        }

        public virtual bool FilteredEntity(string attribute)
        {
            throw new LanguageAttributeNotSupportedException(attribute, this.EntityName);
        }

        [NotMapped]
        public virtual string EntityName
        {
            get
            {
                return this.ToString();
            }
        }
        public LanguageableEntity()
        {
            this.ParentLevelEntityId = null;
        }

        [NotMapped]
        public int? ParentLevelEntityId { get; set; }

        [NotMapped]
        public int? ManagerId { get; set; }
    }
}
