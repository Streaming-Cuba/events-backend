using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models
{
    [Index(nameof(EventId), nameof(TagId), IsUnique = true)]
    public class EventTag : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public Event Event { get; set; }

        public NTag Tag { get; set; }

        public int EventId { get; set; }

        public int TagId { get; set; }
    }
}