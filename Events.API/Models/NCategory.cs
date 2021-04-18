using System.ComponentModel.DataAnnotations;

namespace Events.API.Models
{
    public class NCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}