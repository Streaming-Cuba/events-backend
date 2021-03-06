using System.ComponentModel.DataAnnotations;

namespace Events.API.Models
{
    public class Interaction : BaseModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public bool Like { get; set; } = false;
        
        [Required]
        public bool Love { get; set; } = false;
    }
}