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
            // Find user or company by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user != null && VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                var token = GenerateJwtToken(user.Username, user.Role);
                return Ok(new { Token = token });
            }

            // Optional: Add logic for companies if needed
            return Unauthorized(new { message = "Invalid username or password" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Log the incoming request
            Console.WriteLine($"Register Request: Username={request.Username}, IsCompany={request.IsCompany}, CompanyID={request.CompanyID}");

            // Check if username already exists in users
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Username already exists. Please use a different username." });
            }

            if (request.IsCompany)
            {
                // Check if the company exists using the provided CompanyID
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyID == request.CompanyID);

                if (company == null)
                {
                    return NotFound(new { message = "Company not found. Please provide a valid CompanyID." });
                }

                // Log the company details
                Console.WriteLine($"Company Found: CompanyID={company.CompanyID}, Email={company.Email}");

                // Create a login for the existing company
                using (var hmac = new System.Security.Cryptography.HMACSHA512())
                {
                    var user = new User
                    {
                        Username = request.Username,
                        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                        PasswordSalt = hmac.Key,
                        Role = "Company", // Assign the "Company" role
                        CompanyID = request.CompanyID // Associate the user with the company
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // Log the saved user details
                    Console.WriteLine($"User Created: Username={user.Username}, CompanyID={user.CompanyID}");
                }

                return Ok(new { message = "Login created successfully for the existing company" });
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

                    // Log the saved user details
                    Console.WriteLine($"User Created: Username={user.Username}, Role={user.Role}");
                }

                return Ok(new { message = "User registered successfully" });
            }
        }

        [HttpPost("add-company-login")]
        public async Task<IActionResult> AddCompanyLogin([FromBody] AddCompanyLoginRequest request)
        {
            // Check if the company exists
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Email == request.CompanyEmail);
            if (company == null)
            {
                return NotFound(new { message = "Company not found" });
            }

            // Check if the username already exists
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Create a new user account for the company
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                    PasswordSalt = hmac.Key,
                    Role = "Company" // Assign the "Company" role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Login created successfully for the company" });
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
        public int? CompanyID { get; set; } // Optional CompanyID for existing companies
        public string? CompanyName { get; set; }
        public string? Industry { get; set; }
        public string? CVR { get; set; }
    }

    public class AddCompanyLoginRequest
    {
        public string CompanyEmail { get; set; } = null!; // Email of the existing company
        public string Username { get; set; } = null!;    // New username for the login
        public string Password { get; set; } = null!;    // Password for the new login
    }
}