using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models
{
    public class GroupItemVote
    {
        [Key]
        public int Id { get; set; }

        public int Count { get; set; }

        [Required]
        public string Type { get; set; }
    }
}