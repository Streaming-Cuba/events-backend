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

namespace Events.API.Controllers
{
    [Route("/api/v1/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AccountContext _context;
        private readonly PasswordHasher<string> _passwordHasher;

        public LoginController(IConfiguration configuration,
                                AccountContext context)
        {
            _configuration = configuration;
            _context = context;
            _passwordHasher = new PasswordHasher<string>();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthenticationModel login)
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

        private string GenerateJSONWebToken(Account userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = userInfo.AccountRoles.Select(x => new Claim(ClaimTypes.Role, x.Role.Name))
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
            Account account = await _context.Accounts.Include(d => d.AccountRoles)
                                                     .ThenInclude(p => p.Role)
                                                     .FirstOrDefaultAsync(x => x.Email == login.Email);
            // check for valid authentication
            if (account == null
                || _passwordHasher.VerifyHashedPassword(account.Email,
                                                        account.Password,
                                                        login.Password) != PasswordVerificationResult.Success)
                return null;

            return account;
        }
    }
}