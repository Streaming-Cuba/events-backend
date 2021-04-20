using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Events.API.Models
{
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }

        public int PermissionId { get; set; }

        public int RoleId { get; set; }

        public Permission Permission { get; set; }

        public Role Role { get; set; }
    }
}