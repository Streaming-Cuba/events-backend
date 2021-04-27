using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models
{
    [Index(nameof(Type), IsUnique = true)]
    public class GroupItemVote
    {
        [Key]
        public int Id { get; set; }

        public int Count { get; set; }

        public string Type { get; set; }
    }
}