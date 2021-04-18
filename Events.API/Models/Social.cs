using System.ComponentModel.DataAnnotations;

namespace Events.API.Models
{
    public class Social
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public SocialPlatformType PlatformType { get; set; }

        [Required]
        public string Url { get; set; }
    }
}