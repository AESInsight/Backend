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
            // First try to find a user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user != null && VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                var token = GenerateJwtToken(user.Username, user.Role);
                return Ok(new { Token = token });
            }

            // If no user found, try to find a company
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Email == request.Username);

            if (company != null && BCrypt.Net.BCrypt.Verify(request.Password, company.PasswordHash))
            {
                var token = GenerateJwtToken(company.Email, "Company");
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Check if username/email already exists in either users or companies
            if (await _context.Users.AnyAsync(u => u.Username == request.Username) ||
                await _context.Companies.AnyAsync(c => c.Email == request.Username))
            {
                return BadRequest(new { message = "Username/Email already exists" });
            }

            if (request.IsCompany)
            {
                // Get the next available CompanyID
                var maxCompanyId = await _context.Companies.MaxAsync(c => (int?)c.CompanyID) ?? 0;
                
                // Create company
                var company = new CompanyModel
                {
                    CompanyID = maxCompanyId + 1,
                    CompanyName = request.CompanyName ?? "Default Company Name",
                    Industry = request.Industry ?? "Default Industry",
                    CVR = request.CVR ?? "Default CVR",
                    Email = request.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                // Get the next available UserId
                var maxUserId = await _context.Users.MaxAsync(u => (int?)u.UserId) ?? 0;

                // Also create a user account for the company
                using (var hmac = new System.Security.Cryptography.HMACSHA512())
                {
                    var user = new User
                    {
                        UserId = maxUserId + 1,
                        Username = request.Username,
                        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                        PasswordSalt = hmac.Key,
                        Role = "Company" // Special role for company users
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Company registered successfully" });
            }
            else
            {
                // Get the next available UserId
                var maxUserId = await _context.Users.MaxAsync(u => (int?)u.UserId) ?? 0;

                // Create user
                using (var hmac = new System.Security.Cryptography.HMACSHA512())
                {
                    var user = new User
                    {
                        UserId = maxUserId + 1,
                        Username = request.Username,
                        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                        PasswordSalt = hmac.Key,
                        Role = request.Role
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "User registered successfully" });
            }
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

        private string GenerateJwtToken(string username, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"] ?? "DefaultSecretKey";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
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
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "User";
        public bool IsCompany { get; set; }
        public string? CompanyName { get; set; }
        public string? Industry { get; set; }
        public string? CVR { get; set; }
    }
}