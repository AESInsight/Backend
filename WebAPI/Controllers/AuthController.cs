using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Only check the User table
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user != null && VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                var token = GenerateJwtToken(user.Email, user.Role);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Check if Email already exists in users
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Email already exists as a user" });
            }

            int? companyId = request.CompanyID;

            // Optionally, validate that the company exists if CompanyID is provided
            if (companyId.HasValue && !await _context.Companies.AnyAsync(c => c.CompanyID == companyId.Value))
            {
                return BadRequest(new { message = "Invalid CompanyID" });
            }

            var maxUserId = await _context.Users.MaxAsync(u => (int?)u.UserId) ?? 0;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var user = new User
                {
                    UserId = maxUserId + 1,
                    Email = request.Email,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                    PasswordSalt = hmac.Key,
                    Role = request.Role,
                    CompanyID = companyId // This is null for standalone users, or set for company users
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Registration successful" });
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password), "Password cannot be null.");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }

        private string GenerateJwtToken(string Email, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"] ?? "DefaultSecretKey";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, Email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expireMinutes = double.Parse(jwtSettings["ExpireMinutes"] ?? "30");

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "User";
        public int? CompanyID { get; set; } // Optional: assign to a company if provided
    }
}