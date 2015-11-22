using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BeaversHockeyPortal.Models
{
    public class TeamViewModel
    {
        public IEnumerable<ListTeamViewModel> ListTeamViewModels { get; set; }
        public CreateTeamViewModel CreateTeamViewModel { get; set; }
    }

    public class ListTeamViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Team Name")]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Team manager")]
        public string ManagerFullName { get; set; }
    }

    public class CreateTeamViewModel
    {
        public CreateTeamViewModel()
        {
            this.AvailableManagers = new List<SelectListItem>();
        }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Team Name")]
        public string Name { get; set; }

        public string ManagerId { get; set; }

        public List<SelectListItem> AvailableManagers { get; set; }

    }
}
