using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models
{
    [Index(nameof(AccountId), nameof(RoleId), IsUnique = true)]
    public class AccountRole : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public Account Account { get; set; }

        public Role Role { get; set; }

        public int AccountId { get; set; }

        public int RoleId { get; set; }
    }
}