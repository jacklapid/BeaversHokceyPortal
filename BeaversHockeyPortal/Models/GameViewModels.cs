using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BeaversHockeyPortal.Models
{
    public class GameViewModel
    {
        public GameCreateViewModel GameCreateViewModel { get; set; }

        public IEnumerable<GameListViewModel> GameListViewModel { get; set; }
    }

    public class GameCreateViewModel
    {
        public GameCreateViewModel()
        {
            AllAvailableTeams = new List<SelectListItem>();
            this.AvailableArenas = new List<SelectListItem>();
            this.AvailableTeamsOwnedByManager = new List<SelectListItem>();
            this.AvailableManagers = new List<SelectListItem>();
        }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Game Date")]
        public DateTime Date{ get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Game Time")]
        public DateTime Time { get; set; }

        [Required]
        [Display(Name = "Arena")]
        public int ArenaId{ get; set; }

        [Required]
        [Display(Name = "US (Team)")]
        public int UsId { get; set; }

        [Display(Name = "Them (Team)")]
        public int ThemId { get; set; }

        [Display(Name = "Manager")]
        public string ManagerId { get; set; }

        public List<SelectListItem> AllAvailableTeams { get; set; }

        public List<SelectListItem> AvailableTeamsOwnedByManager { get; set; }

        public List<SelectListItem> AvailableArenas { get; set; }

        public List<SelectListItem> AvailableManagers { get; set; }
    }

public class GameListViewModel
    {
        public GameListViewModel()
        {
            this.GameStatistics = new List<string>();
            this.GameNotes = new List<NoteViewModel>();
        }

        [Display(Name = "Game Id")]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Game Date")]
        [DisplayFormat(DataFormatString = "{0:D}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Game Time")]
        [DisplayFormat(DataFormatString = "{0:HH:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [Display(Name = "Arena")]
        public String Arena{ get; set; }

        [Display(Name = "US (Team)")]
        public string OurTeam{ get; set; }

        [Display(Name = "Opponent")]
        public string TheirTeam { get; set; }

        [Display(Name = "Out Score")]
        public int OurScore { get; set; }

        [Display(Name = "Their Score")]
        public int TheirScore { get; set; }

        public List<String> GameStatistics{ get; set; }

        public List<NoteViewModel> GameNotes { get; set; }

        [Display(Name = "Manager")]
        public string Manager{ get; set; }

        [Display(Name = "Already Confirmed")]
        public string IsConfirmed{ get; set; }

    }
}
