using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public async Task<IActionResult> CreateAccount([FromBody] AccountCreateDTO account)
        {
            // validate data
            if (!account.Email.IsEmail() ||
                (!string.IsNullOrWhiteSpace(account.AvatarPath) && !account.AvatarPath.IsUrl()) ||
                string.IsNullOrWhiteSpace(account.LastName) ||
                string.IsNullOrWhiteSpace(account.Name) ||
                string.IsNullOrWhiteSpace(account.Password))
                return ValidationProblem();

            var role = _context.Roles.FirstOrDefault(x => x.Id == account.RoleId);
            if (role == null)
                return ValidationProblem();

            var _account = _mapper.Map<Account>(account);

            _account.CreatedAt = DateTime.UtcNow;
            _account.ModifiedAt = DateTime.UtcNow;
            _account.Password = _passwordHasher.HashPassword(_account.Email, account.Password);
            // _account.Role = role;

            await _context.Accounts.AddAsync(_account);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("/api/v1/[controller]")]
        public ActionResult<AccountReadDTO> GetAccountByEmail([FromQuery][Required] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return ValidationProblem();

            var account = _context.Accounts.FirstOrDefault(x => x.Email == email);
            if (account != null)
                return _mapper.Map<AccountReadDTO>(account);
            return NotFound(email);
        }


        [Route("/api/v1/[controller]/role")]
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDTO role)
        {
            var _role = _mapper.Map<Role>(role);

            // Check permissions
            if (role.PermissionsId != null)
            {
                foreach (var permissionId in role.PermissionsId)
                {
                    var permission = _context.Permissions.FirstOrDefault(x => x.Id == permissionId);
                    if (permission == null)
                        return ValidationProblem();

                    _context.RolePermissions.Add(new RolePermission
                    {
                        Permission = permission,
                        Role = _role,
                    });
                }
            }

            await _context.Roles.AddAsync(_role);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("/api/v1/[controller]/role")]
        public ActionResult<ICollection<RoleReadDTO>> GetRoles() 
        {
            // Optimize

            Func<Role, RoleReadDTO> converter = (Role x) => {
                var role = _mapper.Map<RoleReadDTO>(x);
                role.PermissionsId = _context.RolePermissions.Where(x => x.RoleId == role.Id).Select(x => x.PermissionId).ToList();
                return role;
            };
            
            return _context.Roles.Select(x => converter(x)).ToArray();            
        }

        [HttpGet]
        [Route("/api/v1/[controller]/role/{id}")]
        public async Task<ActionResult<RoleReadDTO>> GetRoleById([FromRoute] int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound(id);
            var _role = _mapper.Map<RoleReadDTO>(role);
            _role.PermissionsId = _context.RolePermissions.Where(x => x.RoleId == id).Select(x => x.PermissionId).ToList();
            return _role;
        }

        [HttpPost]
        [Route("/api/v1/[controller]/permission")]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionCreateDTO permission)
        {
            var _permission = _mapper.Map<Permission>(permission);

            await _context.Permissions.AddAsync(_permission);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("/api/v1/[controller]/permission")]
        public ActionResult<ICollection<Permission>> GetPermissions() => _context.Permissions.ToArray();

        [HttpGet]
        [Route("/api/v1/[controller]/permission/{id}")]
        public async Task<ActionResult<Permission>> GetPermissionById([FromRoute]int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
                return NotFound(id);
            return permission;
        }
    }
}
