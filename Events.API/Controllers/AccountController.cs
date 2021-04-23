using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Events.API.Data;
using Events.API.DTO;
using Events.API.Helpers;
using Events.API.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

            var role = await _context.Roles.FindAsync(account.RoleId);
            if (role == null)
                return ValidationProblem();

            var _account = _mapper.Map<Account>(account);

            _account.CreatedAt = DateTime.UtcNow;
            _account.ModifiedAt = DateTime.UtcNow;
            _account.Password = _passwordHasher.HashPassword(_account.Email, account.Password);
            _account.Role = role;

            await _context.Accounts.AddAsync(_account);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateAccount), _account.Id);
        }

        [HttpGet]
        [Route("/api/v1/[controller]")]
        public async Task<ActionResult<AccountReadDTO>> GetAccountByEmail([FromQuery][Required] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return ValidationProblem();

            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
            if (account != null)
                return _mapper.Map<AccountReadDTO>(account);
            return NotFound(email);
        }

        [HttpGet]
        [Route("/api/v1/[controller]/{id}")]
        public async Task<ActionResult<AccountReadDTO>> GetAccountById([FromRoute] int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
                return _mapper.Map<AccountReadDTO>(account);
            return NotFound(id);
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
                    var permission = await _context.Permissions.FindAsync(permissionId);
                    if (permission == null)
                        return ValidationProblem();

                    _role.RolePermissions.Add(new RolePermission
                    {
                        Permission = permission,
                        Role = _role,
                    });
                }
            }

            var r = await _context.Roles.AddAsync(_role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateRole), _role.Id);
        }

        [HttpGet]
        [Route("/api/v1/[controller]/role")]
        public ActionResult<ICollection<RoleReadDTO>> GetRoles()
        {
            Func<Role, RoleReadDTO> converter = (Role x) =>
            {
                var role = _mapper.Map<RoleReadDTO>(x);
                role.PermissionsId = x.RolePermissions.Select(x => x.Id).ToArray();
                return role;
            };

            return _context.Roles.Include(x => x.RolePermissions).Select(x => converter(x)).ToArray();
        }

        [HttpGet]
        [Route("/api/v1/[controller]/role/{id}")]
        public async Task<ActionResult<RoleReadDTO>> GetRoleById([FromRoute] int id)
        {
            var role = await _context.Roles.Include(x => x.RolePermissions).FirstOrDefaultAsync(x => x.Id == id);
            if (role == null)
                return NotFound(id);
            var _role = _mapper.Map<RoleReadDTO>(role);
            _role.PermissionsId = role.RolePermissions.Select(x => x.PermissionId).ToArray();
            return _role;
        }

        [HttpPost]
        [Route("/api/v1/[controller]/permission")]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionCreateDTO permission)
        {
            var _permission = _mapper.Map<Permission>(permission);

            await _context.Permissions.AddAsync(_permission);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreatePermission), _permission.Id);
        }

        [HttpGet]
        [Route("/api/v1/[controller]/permission")]
        public ActionResult<ICollection<Permission>> GetPermissions() => _context.Permissions.ToArray();

        [HttpGet]
        [Route("/api/v1/[controller]/permission/{id}")]
        public async Task<ActionResult<Permission>> GetPermissionById([FromRoute] int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
                return NotFound(id);
            return permission;
        }
    }
}
