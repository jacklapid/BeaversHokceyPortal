using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Game
    {
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public Arena  Arena{ get; set; }
        [Required]
        public Team Us { get; set; }
        public Team Them { get; set; }
        public int ScoreUs { get; set; }
        public int ScoreThem { get; set; }
        public ICollection<GameStatistic> GameStatistics { get; set; }
    }
}
