using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}