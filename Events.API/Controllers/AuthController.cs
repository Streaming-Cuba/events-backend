using Events.API.Data;
using Events.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Events.API.DTO;
using AutoMapper;

namespace Events.API.Controllers
{
    [Route("/api/v1/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AccountContext _context;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly IMapper _mapper; 

        public AuthController(IConfiguration configuration,
                              AccountContext context,
                              IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _passwordHasher = new PasswordHasher<string>();
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] AuthenticationModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            IActionResult response = Unauthorized();
            var user = await AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<AccountReadDTO> GetMe()
        {
            var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.Name).Value);
            var account = await _context.Accounts
                .Include(d => d.Roles)
                .ThenInclude(p => p.Role)
                .FirstOrDefaultAsync(x => x.Id == id);

            // FIX: map to relative id of roles
            return _mapper.Map<AccountReadDTO>(account);
        }

        private string GenerateJSONWebToken(Account userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = userInfo.Roles
                .Select(x => new Claim(ClaimTypes.Role, x.Role.Name))
                .Prepend(new Claim(ClaimTypes.Name, userInfo.Id.ToString())).ToArray();

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<Account> AuthenticateUser(AuthenticationModel login)
        {
            var account = await _context.Accounts
                .Include(d => d.Roles)
                .ThenInclude(p => p.Role)
                .FirstOrDefaultAsync(x => x.Email == login.Email);
            // check for valid authentication
            var passwordVerification = _passwordHasher
                .VerifyHashedPassword(account.Email, account.Password, login.Password);
            if (account == null || passwordVerification != PasswordVerificationResult.Success)
                return null;

            return account;
        }
    }
}