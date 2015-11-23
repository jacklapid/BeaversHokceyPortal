using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaversHockeyPortal.Models
{
    public class PlayerViewModels
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        //[Required]
        //public string Email { get; set; }

        public string ImageUrl { get; set; }

        public string Position { get; set; }

        public int PositionId { get; set; }

        public string Team { get; set; }

        public string Manageer{ get; set; }


        //public string Password { get; set; }

        //public string ApplicationUserId { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
