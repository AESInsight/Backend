using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Tests.Controllers
{
    [TestFixture]
    public class TestAuthController
    {
        private ApplicationDbContext _dbContext;
        private AuthController _controller;
        private IConfiguration _configuration;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);

            var inMemorySettings = new Dictionary<string, string?>
            {
                {"Jwt:Key", "TestSecretKeyTestSecretKeyTestSecretKey!!"}, // 32+ chars
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"},
                {"Jwt:ExpireMinutes", "30"}
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _controller = new AuthController(_configuration, _dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task Login_ReturnsOkResult_WithValidCredentials()
        {
            // Arrange
            var password = "TestPassword123";
            var email = "testuser@example.com";
            var role = "User";
            byte[] passwordHash, passwordSalt;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            var user = new User
            {
                UserId = 1,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = role,
                CompanyID = 42
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            // Act
            var result = await _controller.Login(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var token = result.Value.GetType().GetProperty("Token")?.GetValue(result.Value, null) as string;
            var companyId = result.Value.GetType().GetProperty("CompanyID")?.GetValue(result.Value, null);
            Assert.That(token, Is.Not.Null.And.Not.Empty);
            Assert.That(companyId, Is.EqualTo(42));
        }

        [Test]
        public async Task Login_ReturnsUnauthorized_WithInvalidPassword()
        {
            // Arrange
            var password = "TestPassword123";
            var email = "testuser@example.com";
            byte[] passwordHash, passwordSalt;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

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

            var request = new LoginRequest
            {
                Email = email,
                Password = "WrongPassword"
            };

            // Act
            var result = await _controller.Login(request) as UnauthorizedResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task Login_ReturnsUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "AnyPassword"
            };

            // Act
            var result = await _controller.Login(request) as UnauthorizedResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task Register_ReturnsOkResult_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "newuser@example.com",
                Password = "TestPassword123",
                Role = "User",
                CompanyID = null
            };

            // Act
            var result = await _controller.Register(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Registration successful"));
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
                Password = "AnyPassword",
                Role = "User"
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
                Password = "TestPassword123",
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
        public void VerifyPasswordHash_ReturnsTrue_ForCorrectPassword()
        {
            // Arrange
            var password = "TestPassword123";
            byte[] passwordHash, passwordSalt;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            // Use reflection to access the private method
            var method = typeof(AuthController).GetMethod("VerifyPasswordHash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(method, Is.Not.Null, "VerifyPasswordHash method not found via reflection.");

            // Act
            var invokeResult = method!.Invoke(_controller, new object[] { password, passwordHash, passwordSalt });
            Assert.That(invokeResult, Is.Not.Null, "Invoke returned null.");
            var result = (bool)invokeResult;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyPasswordHash_ReturnsFalse_ForIncorrectPassword()
        {
            // Arrange
            var password = "TestPassword123";
            var wrongPassword = "WrongPassword";
            byte[] passwordHash, passwordSalt;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            // Use reflection to access the private method
            var method = typeof(AuthController).GetMethod("VerifyPasswordHash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(method, Is.Not.Null, "VerifyPasswordHash method not found via reflection.");

            // Act
            var invokeResult = method!.Invoke(_controller, new object[] { wrongPassword, passwordHash, passwordSalt });
            Assert.That(invokeResult, Is.Not.Null, "Invoke returned null.");
            var result = (bool)invokeResult!;

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyPasswordHash_ThrowsArgumentNullException_WhenPasswordIsNull()
        {
            // Arrange
            string password = null!;
            byte[] passwordHash = new byte[64];
            byte[] passwordSalt = new byte[128];

            // Use reflection to access the private method
            var method = typeof(AuthController).GetMethod("VerifyPasswordHash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(method, Is.Not.Null, "VerifyPasswordHash method not found via reflection.");

            // Act & Assert
            var ex = Assert.Throws<System.Reflection.TargetInvocationException>(() =>
                method!.Invoke(_controller, new object[] { password, passwordHash, passwordSalt })
            );
            Assert.That(ex.InnerException, Is.TypeOf<ArgumentNullException>());
            Assert.That(ex.InnerException.Message, Does.Contain("Password cannot be null."));
        }
    }
}