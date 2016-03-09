using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    [Table("EmailEvents")]

    public class EmailEvent
    {
        public EmailEvent()
        {
            EmailEventTypes = new List<EmailEventType>();
            EmailTemplates = new List<EmailTemplate>();
        }

        public int Id { get; set; }

        public ICollection<EmailEventType> EmailEventTypes { get; set; }

        public ICollection<EmailTemplate> EmailTemplates { get; set; }

        public Manager Manager { get; set; }

    }
}
