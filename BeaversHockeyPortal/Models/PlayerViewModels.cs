using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BeaversHockeyPortal.Models
{
    public class PlayerRegistrationViewModel
    {
        public PlayerRegistrationViewModel()
        {
            this.AvailableManagers = new List<SelectListItem>();
            this.AvailableTeams = new List<SelectListItem>();
        }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Team")]
        public int TeamId { get; set; }

        [Required]
        [Display(Name = "Manager")]
        public int ManagerId { get; set; }

        public List<SelectListItem> AvailableTeams { get; set; }

        public List<SelectListItem> AvailableManagers { get; set; }
    }


    public class PlayerViewModels
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        //[Required]
        //public string Email { get; set; }

        public string ImageUrl { get; set; }

        public string Position { get; set; }

        public string Status { get; set; }

        public int PositionId { get; set; }

        public string Team { get; set; }

        public string Manageer { get; set; }


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
