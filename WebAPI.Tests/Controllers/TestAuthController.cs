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

        [Test]
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "newuser@example.com",
                Password = "password",
                Role = "User",
                CompanyID = null
            };

            // Act
            var result = await _controller.Register(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(response, Is.EqualTo("Registration successful"));

            // Also check that the user was added to the database
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            Assert.That(user, Is.Not.Null);
            Assert.That(user.Email, Is.EqualTo(request.Email));
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
        {
            // Arrange
            var existingUser = new User
            {
                UserId = 1,
                Email = "existing@example.com",
                PasswordHash = new byte[64],
                PasswordSalt = new byte[128],
                Role = "User"
            };
            _dbContext.Users.Add(existingUser);
            await _dbContext.SaveChangesAsync();

            var request = new RegisterRequest
            {
                Email = "existing@example.com",
                Password = "password",
                Role = "User",
                CompanyID = null
            };

            // Act
            var result = await _controller.Register(request) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Email already exists as a user"));
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenCompanyIdIsInvalid()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "userwithcompany@example.com",
                Password = "password",
                Role = "User",
                CompanyID = 999 // Non-existent company
            };

            // Act
            var result = await _controller.Register(request) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Invalid CompanyID"));
        }

        [Test]
        public void VerifyPasswordHash_ReturnsTrue_WhenPasswordIsCorrect()
        {
            // Arrange
            var password = "securepassword";
            byte[] passwordHash, passwordSalt;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            // Use reflection to call the private method
            var method = typeof(AuthController).GetMethod("VerifyPasswordHash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(method, Is.Not.Null, "VerifyPasswordHash method not found via reflection.");

            // Act
            var invokeResult = method.Invoke(_controller, new object[] { password, passwordHash, passwordSalt });
            Assert.That(invokeResult, Is.Not.Null, "Invoke returned null.");
            var result = (bool)invokeResult;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyPasswordHash_ReturnsFalse_WhenPasswordIsIncorrect()
        {
            // Arrange
            var correctPassword = "securepassword";
            var wrongPassword = "wrongpassword";
            byte[] passwordHash, passwordSalt;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(correctPassword));
            }

            // Use reflection to call the private method
            var method = typeof(AuthController).GetMethod("VerifyPasswordHash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(method, Is.Not.Null, "VerifyPasswordHash method not found via reflection.");

            // Act
            var invokeResult = method.Invoke(_controller, new object[] { wrongPassword, passwordHash, passwordSalt });
            Assert.That(invokeResult, Is.Not.Null, "Invoke returned null.");
            var result = (bool)invokeResult;

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyPasswordHash_ThrowsArgumentNullException_WhenPasswordIsNull()
        {
            // Arrange
            string? password = null;
            byte[] passwordHash = new byte[64];
            byte[] passwordSalt = new byte[128];

            var method = typeof(AuthController).GetMethod("VerifyPasswordHash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(method, Is.Not.Null, "VerifyPasswordHash method not found via reflection.");

            // Act & Assert
#pragma warning disable CS8625 // Suppress possible null reference warning for test purpose
            var ex = Assert.Throws<System.Reflection.TargetInvocationException>(() =>
                method!.Invoke(_controller!, new object?[] { password, passwordHash, passwordSalt })
            );
#pragma warning restore CS8625
            Assert.That(ex!.InnerException, Is.TypeOf<ArgumentNullException>());
            Assert.That(ex.InnerException!.Message, Does.Contain("Password cannot be null."));
        }
    }
}