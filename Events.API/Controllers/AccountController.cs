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
    [Route("/api/v1/account")]
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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountCreateDTO account)
        {
            // validate data
            if (!account.Email.IsEmail() ||
                (!string.IsNullOrWhiteSpace(account.AvatarPath) && !account.AvatarPath.IsUrl()) ||
                string.IsNullOrWhiteSpace(account.Password))
                return ValidationProblem();

            if ((await _context.Accounts.FirstOrDefaultAsync(x => x.Email == account.Email))
                != null)
                return BadRequest(new
                {
                    error = "Already exists a account with this email"
                });                

            var _account = _mapper.Map<Account>(account);

            _account.CreatedAt = DateTime.UtcNow;
            _account.ModifiedAt = DateTime.UtcNow;
            _account.Password = _passwordHasher.HashPassword(_account.Email, account.Password);
            _account.Roles = new List<AccountRole>();

            if (account.RolesId.Count == 0)
                return BadRequest(new
                {
                    error = $"The account must have least one role"
                });

            foreach (var roleId in account.RolesId)
            {
                var role = await _context.Roles.FindAsync(roleId);
                if (role == null)
                    return BadRequest(new
                    {
                        error = $"The role with id: {roleId} don't exists"
                    });
                _account.Roles.Add(new AccountRole
                {
                    Account = _account,
                    Role = role
                });
            }
            // _account.Role = role;

            await _context.Accounts.AddAsync(_account);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateAccount), new { id = _account.Id });
        }

        [HttpGet]
        public async Task<ActionResult<AccountReadDTO>> GetAccountByEmail([FromQuery][Required] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return ValidationProblem();

            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
            if (account != null)
                return _mapper.Map<AccountReadDTO>(account);
            return NotFound(email);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountReadDTO>> GetAccountById([FromRoute] int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
                return _mapper.Map<AccountReadDTO>(account);
            return NotFound(id);
        }


        [HttpPost("role")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDTO role)
        {
            if ((await _context.Roles.FirstOrDefaultAsync(x => x.Name == role.Name))
                != null)
                return BadRequest(new
                {
                    error = "Already exists a role with this name"
                });
            
            var _role = _mapper.Map<Role>(role);
            await _context.Roles.AddAsync(_role);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateRole), new { id = _role.Id });
        }

        [HttpGet("role")]
        public async Task<ActionResult<ICollection<Role>>> GetRoles() => await _context.Roles.ToArrayAsync();

        [HttpGet("role/{id}")]
        public async Task<ActionResult<Role>> GetRoleById([FromRoute] int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound(id);
            return role;
        }
    }
}
