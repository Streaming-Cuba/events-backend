using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public ICollection<GroupItemVote> Votes { get; set; }

        public ICollection<GroupItemSocial> Socials { get; set; }

        public GroupItemMetadata Metadata { get; set; }

        public GroupItemType Type { get; set; }

        public int? Number { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public Group Group { get; set; }
    }
}