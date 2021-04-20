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

        public ICollection<int> PermissionsId { get; set; }
    }

    public class AccountCreateDTO
    {

        public string Name { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string AvatarPath { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public string Password { get; set; }
    }
}