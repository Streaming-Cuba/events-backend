using System.ComponentModel.DataAnnotations;

namespace Events.API.Models
{
    public class GroupItemVote
    {
        [Key]
        public int Id { get; set; }

        public int Count { get; set; }

        public GroupItemVoteType Type { get; set; }
    }
}