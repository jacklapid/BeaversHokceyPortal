using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataModel
{
    [Table("Players")]
    public class Player : Person
    {
        public Player()
        {
            this.UserTypeId = (int)DataModel.Enums.UserTypeEnum.Player;
        }
        [Required]
        public int PlayerStatusId { get; set; }

        public int PlayerPositionId { get; set; }
        [Required]
        public string ManagerId { get; set; }
        public Team Team{ get; set; }
    }
}
