using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Events.API.DTO
{
    public class PermissionCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class RoleCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<int> Permissions { get; set; }
    }
}