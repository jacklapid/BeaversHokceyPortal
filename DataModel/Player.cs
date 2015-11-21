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
            this.UserType_Id = (int)DataModel.Enums.UserTypeEnum.Player;
        }
        [Required]
        public int PlayerStatus_Id { get; set; }

        public int PlayerPosition_Id { get; set; }
        [Required]
        public Manager Manager { get; set; }
        public Team Team{ get; set; }
    }
}
