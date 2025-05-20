using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Tests.Controllers
{
    [TestFixture]
    public class TestAuthController
    {
        private Mock<IConfiguration> _configurationMock;
        private ApplicationDbContext _dbContext;
        private AuthController _controller;

        [SetUp]
        public void SetUp()
        {
            _configurationMock = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthTestDb_" + System.Guid.NewGuid())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _controller = new AuthController(_configurationMock.Object, _dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task Login_ReturnsOkResultWithToken_WhenCredentialsAreValid()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";
            var passwordSalt = new byte[128];
            var passwordHash = new System.Security.Cryptography.HMACSHA512(passwordSalt).ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            var user = new User
            {
                UserId = 1,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(s => s["Key"]).Returns("supersecretkey1234567890superlongkey!");
            jwtSectionMock.Setup(s => s["ExpireMinutes"]).Returns("30");
            jwtSectionMock.Setup(s => s["Issuer"]).Returns("TestIssuer");
            jwtSectionMock.Setup(s => s["Audience"]).Returns("TestAudience");
            _configurationMock.Setup(c => c.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            var request = new LoginRequest { Email = email, Password = password };

            // Act
            var result = await _controller.Login(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.GetType().GetProperty("Token"), Is.Not.Null);
        }

        [Test]
        public async Task Login_ReturnsUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new LoginRequest { Email = "notfound@example.com", Password = "password" };

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
        {
            // Arrange
            var email = "test@example.com";
            var passwordSalt = new byte[128];
            var passwordHash = new System.Security.Cryptography.HMACSHA512(passwordSalt).ComputeHash(System.Text.Encoding.UTF8.GetBytes("correctpassword"));
            var user = new User
            {
                UserId = 1,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new LoginRequest { Email = email, Password = "wrongpassword" };

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }
    }
}