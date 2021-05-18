using System.ComponentModel.DataAnnotations;

namespace Events.API.Models
{
    public class Social : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public SocialPlatformType PlatformType { get; set; }

        [Required]
        public int PlatformTypeId { get; set; }

        [Required]
        public string Url { get; set; }
    }
}