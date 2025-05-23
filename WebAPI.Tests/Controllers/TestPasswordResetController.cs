using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace WebAPI.Tests.Controllers
{
    [TestFixture]
    public class TestPasswordResetController
    {
        private ApplicationDbContext _dbContext;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ICompanyService> _companyServiceMock;
        private Mock<ILogger<PasswordResetController>> _loggerMock;
        private PasswordResetController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _emailServiceMock = new Mock<IEmailService>();
            _companyServiceMock = new Mock<ICompanyService>();
            _loggerMock = new Mock<ILogger<PasswordResetController>>();
            _controller = new PasswordResetController(
                _emailServiceMock.Object,
                _dbContext,
                _loggerMock.Object,
                _companyServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task RequestPasswordReset_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Email = "user@example.com",
                PasswordHash = new byte[32], // or any non-null byte array
                PasswordSalt = new byte[32]
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new PasswordResetRequest { Email = "user@example.com" };

            // Act
            var result = await _controller.RequestPasswordReset(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("If your email is registered, you will receive a password reset link."));
            _emailServiceMock.Verify(s => s.SendPasswordResetEmailAsync("user@example.com", It.IsAny<string>()), Times.Once);

            // Check that the user has a reset token and expiry set
            var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "user@example.com");
            Assert.That(updatedUser, Is.Not.Null, "User should not be null");
            Assert.That(updatedUser.ResetPasswordToken, Is.Not.Null.And.Not.Empty);
            Assert.That(updatedUser.ResetPasswordTokenExpiry, Is.Not.Null);
        }

        [Test]
        public async Task RequestPasswordReset_ReturnsOk_WhenCompanyExists()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company@example.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var request = new PasswordResetRequest { Email = "company@example.com" };

            // Act
            var result = await _controller.RequestPasswordReset(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("If your email is registered, you will receive a password reset link."));
            _emailServiceMock.Verify(s => s.SendPasswordResetEmailAsync("company@example.com", It.IsAny<string>()), Times.Once);

            // Check that the company has a reset token and expiry set
            var updatedCompany = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == "company@example.com");
            Assert.That(updatedCompany, Is.Not.Null, "Company should not be null");
            Assert.That(updatedCompany.ResetPasswordToken, Is.Not.Null.And.Not.Empty);
            Assert.That(updatedCompany.ResetPasswordTokenExpiry, Is.Not.Null);
        }

        [Test]
        public async Task RequestPasswordReset_ReturnsOk_WhenEmailDoesNotExist()
        {
            // Arrange
            var request = new PasswordResetRequest { Email = "nonexistent@example.com" };

            // Act
            var result = await _controller.RequestPasswordReset(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("If your email is registered, you will receive a password reset link."));
            _emailServiceMock.Verify(s => s.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ResetPassword_ReturnsBadRequest_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Token = "sometoken",
                NewPassword = "password1",
                ConfirmPassword = "password2"
            };

            // Act
            var result = await _controller.ResetPassword(request) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            var message = result.Value?.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Passwords do not match"));
        }

        [Test]
        public async Task ResetPassword_ReturnsBadRequest_WhenTokenIsInvalid()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Token = "invalidtoken",
                NewPassword = "password",
                ConfirmPassword = "password"
            };

            // Act
            var result = await _controller.ResetPassword(request) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            var message = result.Value?.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Invalid or expired token"));
        }

        [Test]
        public async Task ResetPassword_ResetsPassword_ForUser()
        {
            // Arrange
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var user = new User
            {
                Email = "user@example.com",
                PasswordHash = new byte[32],
                PasswordSalt = new byte[32],
                ResetPasswordToken = token,
                ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(5)
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new ResetPasswordRequest
            {
                Token = token,
                NewPassword = "newpassword",
                ConfirmPassword = "newpassword"
            };

            // Act
            var result = await _controller.ResetPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var message = result.Value?.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Password has been reset successfully"));

            // Check that the user's token and expiry are cleared
            var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "user@example.com");
            Assert.That(updatedUser?.ResetPasswordToken, Is.Null);
            Assert.That(updatedUser?.ResetPasswordTokenExpiry, Is.Null);
            Assert.That(updatedUser?.PasswordHash, Is.Not.Null.And.Not.Empty);
            Assert.That(updatedUser?.PasswordSalt, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task ResetPassword_ResetsPassword_ForCompany()
        {
            // Arrange
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company@example.com",
                PasswordHash = new byte[32],
                PasswordSalt = new byte[32],
                ResetPasswordToken = token,
                ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(5)
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var request = new ResetPasswordRequest
            {
                Token = token,
                NewPassword = "newpassword",
                ConfirmPassword = "newpassword"
            };

            // Act
            var result = await _controller.ResetPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var message = result.Value?.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Password has been reset successfully"));

            // Check that the company's token and expiry are cleared
            var updatedCompany = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == "company@example.com");
            Assert.That(updatedCompany?.ResetPasswordToken, Is.Null);
            Assert.That(updatedCompany?.ResetPasswordTokenExpiry, Is.Null);
            Assert.That(updatedCompany?.PasswordHash, Is.Not.Null.And.Not.Empty);
            Assert.That(updatedCompany?.PasswordSalt, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task VerifyPassword_ReturnsTrue_ForCorrectUserPassword()
        {
            // Arrange
            var password = "TestPassword123";
            byte[] passwordHash, passwordSalt;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            var user = new User
            {
                Email = "user@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new VerifyPasswordRequest
            {
                Email = "user@example.com",
                Password = password
            };

            // Act
            var result = await _controller.VerifyPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var isValid = result.Value?.GetType().GetProperty("isValid")?.GetValue(result.Value, null);
            Assert.That(isValid, Is.True);
        }

        [Test]
        public async Task VerifyPassword_ReturnsFalse_ForIncorrectUserPassword()
        {
            // Arrange
            var password = "TestPassword123";
            byte[] passwordHash, passwordSalt;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            var user = new User
            {
                Email = "user@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new VerifyPasswordRequest
            {
                Email = "user@example.com",
                Password = "WrongPassword"
            };

            // Act
            var result = await _controller.VerifyPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var isValid = result.Value?.GetType().GetProperty("isValid")?.GetValue(result.Value, null);
            Assert.That(isValid, Is.False);
        }

        [Test]
        public async Task VerifyPassword_ReturnsTrue_ForCorrectCompanyPassword()
        {
            // Arrange
            var password = "CompanyPassword";
            byte[] passwordHash, passwordSalt;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var request = new VerifyPasswordRequest
            {
                Email = "company@example.com",
                Password = password
            };

            // Act
            var result = await _controller.VerifyPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var isValid = result.Value?.GetType().GetProperty("isValid")?.GetValue(result.Value, null);
            Assert.That(isValid, Is.True);
        }

        [Test]
        public async Task VerifyPassword_ReturnsFalse_ForIncorrectCompanyPassword()
        {
            // Arrange
            var password = "CompanyPassword";
            byte[] passwordHash, passwordSalt;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var request = new VerifyPasswordRequest
            {
                Email = "company@example.com",
                Password = "WrongPassword"
            };

            // Act
            var result = await _controller.VerifyPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var isValid = result.Value?.GetType().GetProperty("isValid")?.GetValue(result.Value, null);
            Assert.That(isValid, Is.False);
        }

        [Test]
        public async Task VerifyPassword_ReturnsFalse_WhenUserOrCompanyDoesNotExist()
        {
            // Arrange
            var request = new VerifyPasswordRequest
            {
                Email = "nonexistent@example.com",
                Password = "AnyPassword"
            };

            // Act
            var result = await _controller.VerifyPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var isValid = result.Value?.GetType().GetProperty("isValid")?.GetValue(result.Value, null);
            Assert.That(isValid, Is.False);
        }
    }
}