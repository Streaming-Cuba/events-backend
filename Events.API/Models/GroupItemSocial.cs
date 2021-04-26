using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models
{
    [Index(nameof(ItemId), nameof(SocialId), IsUnique = true)]
    public class GroupItemSocial
    {
        [Key]
        public int Id { get; set; }

        public GroupItem Item { get; set; }

        public Social Social { get; set; }

        public int ItemId { get; set; }

        public int SocialId { get; set; }
    }
}