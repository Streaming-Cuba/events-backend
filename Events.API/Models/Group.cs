using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Events.API.Models
{
    public class Group : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Group> ChildGroups { get; set; }

        public ICollection<GroupItem> Items { get; set; }

        public int? Order { get; set; }

        // backward reference
        [JsonIgnore]
        public int? GroupParentId { get; set; }

        [JsonIgnore]
        public Group GroupParent { get; set; }

        [JsonIgnore]
        public int? EventId { get; set; }

        [JsonIgnore]
        public Event Event { get; set; }
    }
}