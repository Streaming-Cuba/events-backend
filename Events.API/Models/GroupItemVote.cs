using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Events.API.Models
{
    public class GroupItemVote : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public int Count { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [JsonIgnore]
        public int GroupItemId { get; set; }

        [Required]
        [JsonIgnore]
        public GroupItem GroupItem { get; set; }
    }
}