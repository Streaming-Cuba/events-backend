using System.ComponentModel.DataAnnotations;

namespace Events.API.DTO
{
    public class SocialCreateDTO 
    {
        public int? PlatformTypeId { get; set; }

        [Required]
        public string Url { get; set; }
    }
}