﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class PlayerStatus
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
