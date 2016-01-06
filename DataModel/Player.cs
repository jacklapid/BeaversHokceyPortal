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
        public Manager Manager    { get; set; }
        public Team Team{ get; set; }

        [NotMapped]
        public bool IsRegular
        {
            get { return this.PlayerStatus_Id == (int)Enums.PlayerStatusEnum.Regular; }
        }

        [NotMapped]
        public bool IsSpare
        {
            get { return this.PlayerStatus_Id == (int)Enums.PlayerStatusEnum.Spare; }
        }
        [NotMapped]
        public bool IsGoalie
        {
            get { return this.PlayerStatus_Id == (int)Enums.PlayerPositionEnum.Goalie; }
        }
        [NotMapped]
        public bool IsPlayer
        {
            get { return this.PlayerStatus_Id == (int)Enums.PlayerPositionEnum.Player; }
        }
    }
}
