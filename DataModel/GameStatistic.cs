using System.ComponentModel.DataAnnotations;

namespace DataModel
{
    public class GameStatistic
    {
        public int Id { get; set; }
        [Required]
        public string Description{ get; set; }
    }
}