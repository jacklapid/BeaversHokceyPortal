using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataModel
{
    [Table("Managers")]
    public class Manager : Person
    {
        //[Required]
        //public ICollection<Team> Teams { get; set; }

        public Manager()
        {
            this.UserType_Id = (int)DataModel.Enums.UserTypeEnum.Manager;
            //Teams = new List<Team>();
            //Players = new List<Player>();
        }
        ////public ICollection<Player> Players { get; set; }
    }
}
