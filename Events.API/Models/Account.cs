using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models {
    [Index(nameof(Email), IsUnique = true)]
    public class Account : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string AvatarPath { get; set; }

        [Required]
        public ICollection<AccountRole> Roles { get; set; }

        [Required]
        public string Password { get; set; }

        public bool Active { get; set; } = false;
    }
}