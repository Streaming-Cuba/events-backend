using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models
{
    public class GroupItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string CoverPath { get; set; }

        public uint Votes { get; set; }

        public ICollection<Social> Socials { get; set; }

        public string Metadata { get; set; }

        public GroupItemType Type { get; set; }

        public int? Number { get; set; }
    }
}