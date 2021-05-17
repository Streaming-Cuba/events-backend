using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models
{
    [Index(nameof(EventId), nameof(SocialId), IsUnique = true)]
    public class EventSocial : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public Event Event { get; set; }

        public Social Social { get; set; }

        public int EventId { get; set; }

        public int SocialId { get; set; }
    }
}