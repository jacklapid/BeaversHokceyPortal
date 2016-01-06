using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class Game
    {
        public Game()
        {
            this.GameNotes = new List<Note>();
            this.GameStatistics = new List<GameStatistic>();
        }
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public Arena Arena { get; set; }

        [Required]
        public Team Us { get; set; }

        public Team Them { get; set; }

        public int ScoreUs { get; set; }

        public int ScoreThem { get; set; }

        public ICollection<GameStatistic> GameStatistics { get; set; }

        public ICollection<Note> GameNotes { get; set; }

        public Manager Manager { get; set; }

        public Season Season { get; set; }

        [NotMapped]
        public  string Name
        {
            get
            {
                return this.Date.ToString();
            }
        }
    }
}
