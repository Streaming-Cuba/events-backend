using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Identifier { get; set; }

        [Required]
        public string Name { get; set; }

        public string Subtitle { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public NEventStatus Status { get; set; }

        public string Organizer { get; set; }

        public NCategory Category { get; set; }

        public ICollection<EventTag> Tags { get; set; }

        public ICollection<EventSocial> Socials { get; set; }

        public string CoverPath { get; set; }

        public string ShortCoverPath { get; set; }
        
        public ICollection<Interation> Interations { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }

}
