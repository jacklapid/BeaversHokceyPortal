using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class EmailEvent
    {
        public EmailEvent()
        {
            EmailEventTypes = new List<EmailEventType>();
            EmailTemplates = new List<EmailTemplate>();

        }
        public int Id { get; set; }

        public ICollection<EmailEventType> EmailEventTypes { get; set; }

        public int DaysBeforeGame { get; set; }

        public int? DaysForReoccurrence { get; set; }

        public ICollection<EmailTemplate> EmailTemplates { get; set; }

        public Manager Manager { get; set; }
    }
}
