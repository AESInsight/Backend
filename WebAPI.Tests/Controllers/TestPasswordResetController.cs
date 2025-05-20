using Backend.Controllers;
using Backend.Data;
using Backend.Models; // <-- Add this for User, CompanyModel, PasswordResetRequest
using Backend.Services;
using Microsoft.AspNetCore.Mvc; // <-- Add this for OkObjectResult
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace WebAPI.Tests.Controllers
{
    [TestFixture]
    public class TestPasswordResetController
    {
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ICompanyService> _companyServiceMock;
        private Mock<ILogger<PasswordResetController>> _loggerMock;
        private ApplicationDbContext _dbContext;
        private PasswordResetController _controller;

        [SetUp]
        public void SetUp()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _companyServiceMock = new Mock<ICompanyService>();
            _loggerMock = new Mock<ILogger<PasswordResetController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "PasswordResetTestDb_" + System.Guid.NewGuid())
                .Options;
            _dbContext = new ApplicationDbContext(options);
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
        public async Task RequestPasswordReset_ReturnsOk_WhenEmailNotRegistered()
        {
            // Arrange
            var request = new PasswordResetRequest { Email = "notfound@example.com" };

            // Act
            var result = await _controller.RequestPasswordReset(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("If your email is registered, you will receive a password reset link."));
            _emailServiceMock.Verify(e => e.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task RequestPasswordReset_SetsTokenAndSendsEmail_ForUser()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Email = "user@example.com",
                PasswordHash = new byte[64],
                PasswordSalt = new byte[128],
                Role = "User"
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new PasswordResetRequest { Email = user.Email };

            // Act
            var result = await _controller.RequestPasswordReset(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            Assert.That(updatedUser!.ResetPasswordToken, Is.Not.Null);
            Assert.That(updatedUser.ResetPasswordTokenExpiry, Is.Not.Null);
            _emailServiceMock.Verify(e => e.SendPasswordResetEmailAsync(user.Email, It.IsAny<string>()), Times.Once);

            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("If your email is registered, you will receive a password reset link."));
        }

        [Test]
        public async Task RequestPasswordReset_SetsTokenAndSendsEmail_ForCompany()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 1,
                Email = "company@example.com",
                CompanyName = "Test Company",
                Industry = "Test Industry",
                CVR = "12345678"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var request = new PasswordResetRequest { Email = company.Email };

            // Act
            var result = await _controller.RequestPasswordReset(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var updatedCompany = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == company.Email);
            Assert.That(updatedCompany!.ResetPasswordToken, Is.Not.Null);
            Assert.That(updatedCompany.ResetPasswordTokenExpiry, Is.Not.Null);
            _emailServiceMock.Verify(e => e.SendPasswordResetEmailAsync(company.Email, It.IsAny<string>()), Times.Once);

            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("If your email is registered, you will receive a password reset link."));
        }

        [Test]
        public async Task ResetPassword_ReturnsOk_WhenUserTokenIsValidAndPasswordsMatch()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Email = "user@example.com",
                PasswordHash = new byte[64],
                PasswordSalt = new byte[128],
                Role = "User",
                ResetPasswordToken = "valid-token",
                ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(10)
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new ResetPasswordRequest
            {
                Token = "valid-token",
                NewPassword = "newpassword",
                ConfirmPassword = "newpassword"
            };

            // Act
            var result = await _controller.ResetPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Password has been reset successfully"));

            var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            Assert.That(updatedUser!.ResetPasswordToken, Is.Null);
            Assert.That(updatedUser.ResetPasswordTokenExpiry, Is.Null);
        }

        [Test]
        public async Task ResetPassword_ReturnsOk_WhenCompanyTokenIsValidAndPasswordsMatch()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 1,
                Email = "company@example.com",
                CompanyName = "Test Company",
                Industry = "Test Industry",
                CVR = "12345678",
                ResetPasswordToken = "valid-token",
                ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(10)
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var request = new ResetPasswordRequest
            {
                Token = "valid-token",
                NewPassword = "newpassword",
                ConfirmPassword = "newpassword"
            };

            // Act
            var result = await _controller.ResetPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Password has been reset successfully"));

            var updatedCompany = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == company.Email);
            Assert.That(updatedCompany!.ResetPasswordToken, Is.Null);
            Assert.That(updatedCompany.ResetPasswordTokenExpiry, Is.Null);
        }

        [Test]
        public async Task ResetPassword_ReturnsBadRequest_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Token = "any-token",
                NewPassword = "password1",
                ConfirmPassword = "password2"
            };

            // Act
            var result = await _controller.ResetPassword(request) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Passwords do not match"));
        }

        [Test]
        public async Task ResetPassword_ReturnsBadRequest_WhenTokenIsInvalidOrExpired()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Email = "user@example.com",
                PasswordHash = new byte[64],
                PasswordSalt = new byte[128],
                Role = "User",
                ResetPasswordToken = "expired-token",
                ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(-10)
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new ResetPasswordRequest
            {
                Token = "expired-token",
                NewPassword = "newpassword",
                ConfirmPassword = "newpassword"
            };

            // Act
            var result = await _controller.ResetPassword(request) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Invalid or expired token"));
        }

        [Test]
        public async Task VerifyPassword_ReturnsTrue_WhenUserPasswordIsCorrect()
        {
            // Arrange
            var password = "correctpassword";
            byte[] passwordSalt, passwordHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            var user = new User
            {
                UserId = 1,
                Email = "user@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User"
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new VerifyPasswordRequest { Email = user.Email, Password = password };

            // Act
            var result = await _controller.VerifyPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var isValid = (bool)result.Value!.GetType().GetProperty("isValid")!.GetValue(result.Value, null)!;
            Assert.That(isValid, Is.True);
        }

        [Test]
        public async Task VerifyPassword_ReturnsFalse_WhenUserPasswordIsIncorrect()
        {
            // Arrange
            var password = "correctpassword";
            byte[] passwordSalt, passwordHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            var user = new User
            {
                UserId = 1,
                Email = "user@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User"
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new VerifyPasswordRequest { Email = user.Email, Password = "wrongpassword" };

            // Act
            var result = await _controller.VerifyPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var isValid = (bool)result.Value!.GetType().GetProperty("isValid")!.GetValue(result.Value, null)!;
            Assert.That(isValid, Is.False);
        }

        [Test]
        public async Task VerifyPassword_ReturnsTrue_WhenCompanyPasswordIsCorrect()
        {
            // Arrange
            var password = "companypass";
            byte[] passwordSalt, passwordHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            var company = new CompanyModel
            {
                CompanyID = 1,
                Email = "company@example.com",
                CompanyName = "Test Company",
                Industry = "Test Industry",
                CVR = "12345678",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var request = new VerifyPasswordRequest { Email = company.Email, Password = password };

            // Act
            var result = await _controller.VerifyPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var isValid = (bool)result.Value!.GetType().GetProperty("isValid")!.GetValue(result.Value, null)!;
            Assert.That(isValid, Is.True);
        }

        [Test]
        public async Task VerifyPassword_ReturnsFalse_WhenEmailNotFound()
        {
            // Arrange
            var request = new VerifyPasswordRequest { Email = "notfound@example.com", Password = "any" };

            // Act
            var result = await _controller.VerifyPassword(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var isValid = (bool)result.Value!.GetType().GetProperty("isValid")!.GetValue(result.Value, null)!;
            Assert.That(isValid, Is.False);
        }
    }
}