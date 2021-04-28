using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Subscriber
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Institution { get; set; }

        [Required]
        public string Email { get; set; }
    }
}