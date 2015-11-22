using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaversHockeyPortal.Models
{
    public class NoteViewModel
    {
        [DisplayFormat(DataFormatString = "F")]
        public DateTime TimeStamp{ get; set; }

        public string Text { get; set; }

        public string Creator{ get; set; }
    }
}
