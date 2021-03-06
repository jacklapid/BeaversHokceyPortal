﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    [Table("Persons")]
    public partial class Person
    {
        public Person()
        {

        }
        public int Id { get; set; }

        [Required]
        public int UserType_Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        //[Required]
        //public string Email { get; set; }

        public FileAttachment Image { get; set; }

        //public string Password { get; set; }

        public string ApplicationUser_Id { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        [NotMapped]
        public string Name
        {
            get
            {
                return this.FullName;
            }
        }
    }
}
