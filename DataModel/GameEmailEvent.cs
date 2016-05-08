using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    [Table("GameEmailEvents")]

    public class GameEmailEvent : EmailEvent
    {
        public GameEmailEvent() : base()
        {
        }
        public int DaysBeforeGame { get; set; }

        public int? DaysForReoccurrence { get; set; }
    }
}
