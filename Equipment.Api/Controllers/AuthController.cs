using Equipment.Api.Data;
using Equipment.Api.DTO;
using Equipment.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Equipment.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == loginDto.Login && u.Password == loginDto.Password);

            if (user == null)
                return Unauthorized(new { message = "Неверный логин или пароль." });

            var token = GenerateToken(user.Id, user.Role_id);

            var session = new Session
            {
                Token = token,
                UserId = user.Id
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(new { token, roleId = user.Role_id, login = user.Login });
        }
        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            string? token = Request.Headers["Autorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(token))
            {
                var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token);
                if (session != null)
                {
                    _context.Sessions.Remove(session);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok();
        }

        private string GenerateToken(int userId, int roleId)
        {
            var secretKey = _configuration["Jwt:Key"] ?? throw new Exception("JWT НЕ НАЙДЕН!!!!");

            var claims = new[]
            {
                new Claim("userId", userId.ToString()),
                new Claim("roleId", roleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
