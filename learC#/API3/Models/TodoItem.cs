using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lesson3.Models
{
    public class TodoItem
    {
        public int Id{get; set;}
        
        [Required]
        public string Title{get; set;}

        public bool IsCompleted{get; set;}
        public DateTime CreatedAt{get; set;}
        public int UserId{get; set;}

    }
}