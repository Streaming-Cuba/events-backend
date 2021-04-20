using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Events.API.Data;
using Events.API.DTO;
using Events.API.Models;
using Events.API.Services.CDN;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountContext _context;
        private readonly IMapper _mapper;

        public AccountController(AccountContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("/api/v1/[controller]")]
        public async Task<IActionResult> CreateAccount()
        {
            // _context.Accounts.Add()
            return Ok();
        }

        [Route("/api/v1/[controller]/role")]
        public async Task<IActionResult> CreateRole([FromBody]RoleCreateDTO role)
        {
            var _role = _mapper.Map<Role>(role);
            
            // Check permissions
            if (role.Permissions != null) {
                foreach (var permissionId in role.Permissions) 
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
            return CreatedAtAction(nameof(CreateRole), _role);
        }

        [HttpPost]
        [Route("/api/v1/[controller]/permission")]
        public async Task<IActionResult> CreatePermission([FromBody]PermissionCreateDTO permission)
        {           
            var _permission = _mapper.Map<Permission>(permission);
            
            _context.Permissions.Add(_permission);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreatePermission), _permission);
        }
    }
}
