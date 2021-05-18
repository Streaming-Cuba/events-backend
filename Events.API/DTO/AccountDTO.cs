using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Events.API.Models;

namespace Events.API.DTO
{
    public class AccountCreateDTO : CreateModelDTO
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string AvatarPath { get; set; }

        [Required]
        public ICollection<int> RolesId { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AccountReadDTO
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string AvatarPath { get; set; }

        public ICollection<int> RolesId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }

    public class RoleCreateDTO : CreateModelDTO
    {
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
    }
}