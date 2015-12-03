using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel
{
    public class GameStatistic
    {
        public int Id { get; set; }
        [Required]
        public string Description{ get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                return this.Description;
            }
        }
    }
}