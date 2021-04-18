using System.ComponentModel.DataAnnotations;

namespace Events.API.Models
{
    public class SocialPlatformType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }
    }
}