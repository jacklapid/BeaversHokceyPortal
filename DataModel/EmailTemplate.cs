using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class EmailTemplate : INamableEntity
    {
        public EmailTemplate()
        {
            ToPersons = new List<Person>();
            ToUserTypes = new List<UserType>();
            ToPlayerStatuses = new List<PlayerStatus>();
        }
        public int Id { get; set; }

        public string From{ get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Context { get; set; }

        public bool AggrigateLanguageResults { get; set; }

        public ICollection<Person> ToPersons{ get; set; }

        public ICollection<UserType> ToUserTypes { get; set; }

        public ICollection<PlayerStatus> ToPlayerStatuses { get; set; }

        public Manager Manager { get; set; }

        public string Name
        {
            get
            {
                return this.Subject;
            }
        }

        public bool SendToSpecificUsers
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.To);
            }
        }
    }

}
