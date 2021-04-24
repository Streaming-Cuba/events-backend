using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Events.API.DTO
{
    public class AccountCreateDTO
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
}