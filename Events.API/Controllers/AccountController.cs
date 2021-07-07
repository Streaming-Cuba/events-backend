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
using Microsoft.AspNetCore.JsonPatch;
using Events.API.Services;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Events.API.Controllers
{
    [Route("/api/v1/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountContext _context;
        private readonly IMapper _mapper;
        private readonly EmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<string> _passwordHasher;

        public AccountController(AccountContext context,
                                 IConfiguration configuration,
                                 EmailSender emailSender,
                                 IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = new PasswordHasher<string>();
            _configuration = configuration;
            _emailSender = emailSender;
        }        

        private string GenerateJSONWebToken(Account user, short expires = 120)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = user.Roles
                .Select(x => new Claim(ClaimTypes.Role, x.Role.Name))
                .Prepend(new Claim(ClaimTypes.Name, user.Id.ToString()))
                .Prepend(new Claim(ClaimTypes.Email, user.Email)).ToArray();

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(expires),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountCreateDTO account)
        {
            // validate data
            if (!account.Email.IsEmail() ||
                (!string.IsNullOrWhiteSpace(account.AvatarPath) && !account.AvatarPath.IsUrl()))
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
            _account.Password = string.Empty;
            _account.Active = false;
            // _account.Password = _passwordHasher.HashPassword(_account.Email, account.Password);
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

            await _context.Accounts.AddAsync(_account);          
            var token = GenerateJSONWebToken(_account, expires: 1440);

            try 
            {
                var url = $"https://admin.streamingcuba.com/confirm-account?token={token}";
                await _emailSender.SendEmailAsync(account.Email,
                                                  subject: "[StreamingCuba Team]",
                                                  message: $"{url}");

                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(CreateAccount), new { id = _account.Id });
            } 
            catch 
            {
                return StatusCode(501); // Bad Gateway (Unable to send reset password link)
            }            
        }

        [Authorize(Roles = "Administrador")]
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchAccount([FromRoute] int id,
                                                     [FromBody] JsonPatchDocument<AccountCreateDTO> account)
        {
            // validate data
            var _account = await _context.Accounts.Include(d => d.Roles)
                                                  .FirstOrDefaultAsync(x => x.Id == id);
            if (_account == null)
                return NotFound(id);

            // [TODO]: Only check changed values
            await account.ApplyTo(_account, _context, _mapper, ModelState, s =>
            {
                // Manual sync with Roles
                if (s.RolesId != null)
                {
                    foreach (var roleId in s.RolesId)
                    {
                        if (!_account.Roles.Any(x => x.RoleId == roleId))
                            _account.Roles.Add(new AccountRole
                            {
                                RoleId = roleId,
                                AccountId = id
                            });
                    }
                    var toRemove = _account.Roles.Where(x => !s.RolesId.Contains(x.RoleId)).ToList();
                    foreach (var item in toRemove)
                        _account.Roles.Remove(item);
                }
            });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // rehash password
            _account.Password = _passwordHasher.HashPassword(_account.Email, _account.Password);
            _account.ModifiedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public ActionResult<IEnumerable<AccountReadDTO>> GetAccounts([FromQuery] string email)
            => Ok(email != null ? _context.Accounts.Include(d => d.Roles)
                                                   .ThenInclude(d => d.Role)
                                                   .Where(x => x.Email == email)?
                                                   .Select(x => _mapper.Map<AccountReadDTO>(x)) 
                                : _context.Accounts.Include(d => d.Roles)
                                                   .ThenInclude(d => d.Role)
                                                   .Select(x => _mapper.Map<AccountReadDTO>(x)));

        [Authorize(Roles = "Administrador")]
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountReadDTO>> GetAccountById([FromRoute] int id)
        {
            var account = await _context.Accounts.Include(d => d.Roles)
                                                 .ThenInclude(d => d.Role)
                                                 .FirstOrDefaultAsync(x => x.Id == id);
            if (account != null)
                return _mapper.Map<AccountReadDTO>(account);
            return NotFound(id);
        }

        [HttpPost("role")]
        [Authorize(Roles = "Administrador")]
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
