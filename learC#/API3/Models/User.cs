using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lesson3.Models
{
    public class User
    {
        public int Id{get; set;}
        
        [Required]
        [MinLength(3)]
        public string Username{get; set;}

        public string Password{get; set;}
    }
}