using Equipment.Api.Data;
using Equipment.Api.DTO;
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

        // POST api/auth/login — авторизация пользователя
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Ищем пользователя по логину и паролю
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == loginDto.Login && u.Password == loginDto.Password);

            // Если не нашли — возвращаем ошибку
            if (user == null)
                return Unauthorized(new { message = "Неверный логин или пароль." });

            // Генерируем JWT токен
            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        // Метод генерации JWT токена
        private string GenerateJwtToken(Models.User user)
        {
            var key = _configuration["Jwt:Key"]!;

            // Добавляем в токен информацию о пользователе
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("userId", user.Id.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24), // Токен действует 24 часа
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
