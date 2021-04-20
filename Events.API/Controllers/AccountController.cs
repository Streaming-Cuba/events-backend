using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Events.API.Data;
using Events.API.DTO;
using Events.API.Helpers;
using Events.API.Models;
using Events.API.Services.CDN;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountContext _context;
        private readonly IMapper _mapper;
        private readonly PasswordHasher<string> _passwordHasher;

        public AccountController(AccountContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = new PasswordHasher<string>();
        }

        [HttpPost]
        [Route("/api/v1/[controller]")]
        public async Task<IActionResult> CreateAccount([FromBody]AccountCreateDTO account)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Id == account.RoleId);
            if (role == null)
                return ValidationProblem();
            
            // validate data
            if (!account.Email.IsEmail() ||
                (!string.IsNullOrWhiteSpace(account.AvatarPath) && !account.AvatarPath.IsUrl()) ||
                string.IsNullOrWhiteSpace(account.LastName) ||
                string.IsNullOrWhiteSpace(account.Name) ||
                string.IsNullOrWhiteSpace(account.Password))
                return ValidationProblem();


            var _account = _mapper.Map<Account>(account);

            _account.CreatedAt = DateTime.UtcNow;
            _account.ModifiedAt = DateTime.UtcNow;
            _account.Password = _passwordHasher.HashPassword(_account.Email, account.Password);
            _account.Role = role;

            _context.Accounts.Add(_account);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Route("/api/v1/[controller]/role")]
        public async Task<IActionResult> CreateRole([FromBody]RoleCreateDTO role)
        {
            var _role = _mapper.Map<Role>(role);
            
            // Check permissions
            if (role.PermissionsId != null) {
                foreach (var permissionId in role.PermissionsId) 
                {
                    var permission = _context.Permissions.FirstOrDefault(x => x.Id == permissionId);
                    if (permission == null)
                        return ValidationProblem();

                    _context.RolePermissions.Add(new RolePermission{
                        Permission = permission,
                        Role = _role,
                    });
                }
            }

            _context.Roles.Add(_role);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("/api/v1/[controller]/permission")]
        public async Task<IActionResult> CreatePermission([FromBody]PermissionCreateDTO permission)
        {           
            var _permission = _mapper.Map<Permission>(permission);
            
            _context.Permissions.Add(_permission);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
