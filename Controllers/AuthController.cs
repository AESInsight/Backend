using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
public IActionResult Login([FromBody] LoginRequest request)
{
    // Dummy brugerautentifikation (udskift med din egen logik)
    if (request.Username == "admin" && request.Password == "password")
    {
        var token = GenerateJwtToken(request.Username, "Admin"); // Tilføj rollen her
        return Ok(new { Token = token });
    }

    if (request.Username == "user" && request.Password == "password")
    {
        var token = GenerateJwtToken(request.Username, "User"); // Tilføj rollen her
        return Ok(new { Token = token });
    }

    return Unauthorized();
}

                private string GenerateJwtToken(string username, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, role), // Tilføj rollen som claim
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
        
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"])),
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
}