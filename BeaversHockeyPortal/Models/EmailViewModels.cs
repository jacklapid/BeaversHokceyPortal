using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BeaversHockeyPortal.Models
{
    public enum ToSelection
    {
        Specific = 1,
        Text
    }

    public enum EmailEventType
    {
        Game = 1,
        Other
    }

    public class EmailEventViewModel
    {
        [Display(Name = "Email Events")]
        public int[] EventTypes{ get; set; }
        public IEnumerable<SelectListItem> AvailableEventTypes { get; set; }

        [Display(Name = "Email Events")]
        public string EventTypesString { get; set; }

        [Display(Name = "Number of days before the game")]
        public int DaysBeforeGame { get; set; }

        [Display (Name = "Repeat every N-days")]
        public int? ReoccurrenceDays { get; set; }
        [Display(Name = "Repeat every N-days")]
        public string ReoccurrenceDaysString { get; set; }

        [Display(Name = "Email templates")]
        [Required]
        public int[] EmailTemplates { get; set; }
        public IEnumerable<SelectListItem> AvailableEmailTemplates { get; set; }
        [Display(Name = "Email Templates")]
        public string EmailTemplatesString { get; set; }

        [Display(Name = "Manager")]
        public string ManagerName { get; set; }
        [Display(Name = "Manager")]
        [Required]
        public int ManagerId { get; set; }
        public List<SelectListItem> AvailableManagers { get; set; }

        [Display(Name = "Email Event Type")]
        public EmailEventType EmailEventType { get; set; }

    }

    public class EmailTemplateViewModel
    {
        [Display(Name = "Context")]
        public string Context { get; set; }

        [Display(Name = "To", Description = "Can be an email address or language fragment")]
        public string To { get; set; }

        [Display(Name = "From")]
        public string From { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [AllowHtml]
        [Display(Name = "Body")]
        public string Body { get; set; }

        [Display(Name = "Aggrigate Language Results", Description = "If there is formula that produces multiples")]
        public bool AggrigateLanguageResults { get; set; }

        [Display(Name = "Users")]
        public int[] ToUserIds { get; set; }
        public IEnumerable<SelectListItem> AvailableUsers { get; set; }

        [Display(Name = "User Types")]
        public int[] ToUserTypeIds { get; set; }
        public IEnumerable<SelectListItem> AvailableUserTypes { get; set; }

        [Display(Name = "Player Statuses")]
        public int[] ToPlayerStatusIds { get; set; }
        public IEnumerable<SelectListItem> AvailablePlayerStatuses { get; set; }

        [Display(Name = "Manager")]
        [Required]
        public int ManagerId { get; set; }
        public List<SelectListItem> AvailableManagers { get; set; }

        [Display(Name = "Selection For 'TO' Field")]
       public ToSelection ToSelection { get; set; }
        //{
        //    get { return string.IsNullOrWhiteSpace(this.To); }
        //}
    }
}
