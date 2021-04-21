using System;
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

    public class RoleReadDTO
    {
        public string Id { get; set; }

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

    public class AccountReadDTO
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string AvatarPath { get; set; }

        public int RoleId { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}