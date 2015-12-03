using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
   public class Note
    {
        public int Id { get; set; }

        public string NoteDescription{ get; set; }

        public DateTime TimeStamp{ get; set; }
        public Person Creator { get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                return this.NoteDescription;
            }
        }
    }
}
