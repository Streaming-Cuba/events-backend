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
using System.ComponentModel.DataAnnotations;
using Events.API.Services;

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
        private readonly EmailSender _emailSender;

        public AuthController(IConfiguration configuration,
                              AccountContext context,
                              EmailSender emailSender,
                              IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _passwordHasher = new PasswordHasher<string>();
            _emailSender = emailSender;
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
                if (!user.Active) 
                {
                    return BadRequest("Unable to sign in with inactive user");
                }
                
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

            return _mapper.Map<AccountReadDTO>(account);
        }

        [HttpPost("confirm-account")]
        [Authorize]
        public async Task<IActionResult> ConfirmAccount() 
        {
            var user = int.Parse(User.Identity.Name);
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == user);
            if (account == null) 
            {
                return NotFound($"Unable to load user");
            }
            
            account.Active = true;
            await _context.SaveChangesAsync();
            return Ok();
        }        


        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromQuery][Required] string email) 
        {
            var user = await _context.Accounts.Include(d => d.Roles)
                                              .ThenInclude(p => p.Role)
                                              .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null) 
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            var token = GenerateJSONWebToken(user, expires: 1440);

            try 
            {
                var url = $"https://events.streamingcuba.com/reset-password?token={token}";
                await _emailSender.SendEmailAsync(email,
                                                  subject: "[StreamingCuba Team] Restablecer contraseña",
                                                  message: $"<a href={url}>{url}</a>");
                return Ok();                
            } 
            catch 
            {
                return StatusCode(501); // Bad Gateway (Unable to send reset password link)
            }
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

        private async Task<Account> AuthenticateUser(AuthenticationModel login)
        {
            var account = await _context.Accounts
                .Include(d => d.Roles)
                .ThenInclude(p => p.Role)
                .FirstOrDefaultAsync(x => x.Email == login.Email);
            if (account == null)
                return null;

            // check for valid authentication
            var passwordVerification = _passwordHasher
                .VerifyHashedPassword(account.Email, account.Password, login.Password);
            if (passwordVerification != PasswordVerificationResult.Success)
                return null;

            return account;
        }
    }
}