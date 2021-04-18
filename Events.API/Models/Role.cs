using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Events.API.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<Permission> Permissions { get; set; }
    }
}