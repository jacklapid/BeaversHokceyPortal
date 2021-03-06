﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Team
    {
        public Team()
        {
            ImageIds = new List<int>();
        }
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<int> ImageIds { get; set; }

        public Manager Manager { get; set; }
    }
}
