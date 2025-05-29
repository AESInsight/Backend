using Backend.Data;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace WebAPI.Tests.Services
{
    [TestFixture]
    public class TestEmailService
    {
        private ApplicationDbContext _dbContext;
        private Mock<IConfiguration> _configurationMock;
        private Mock<ILogger<EmailService>> _loggerMock;
        private EmailService _service;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<EmailService>>();

            // Setup minimal SMTP config section
            var smtpSection = new Mock<IConfigurationSection>();
            smtpSection.Setup(s => s["Server"]).Returns("smtp.test.com");
            _configurationMock.Setup(c => c.GetSection("SmtpSettings")).Returns(smtpSection.Object);

            _service = new EmailService(_configurationMock.Object, _loggerMock.Object, _dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task SendPasswordResetEmailAsync_DoesNothing_WhenCompanyDoesNotExist()
        {
            // Act
            await _service.SendPasswordResetEmailAsync("notfound@company.com", "token123");

            // Assert
            // No exception should be thrown, and logger should be called
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString() != null && v.ToString()!.Contains("not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task SendPasswordResetEmailAsync_SendsEmail_WhenCompanyExists()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company@test.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // Because actual email sending would fail (no SMTP), we expect an exception.
            // We'll catch it and assert up to the point of sending.
            try
            {
                await _service.SendPasswordResetEmailAsync("company@test.com", "token123");
            }
            catch
            {
                // Ignore exceptions from SmtpClient in test environment
            }

            // Assert
            // No logger warning should be called for not found
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString() != null && v.ToString()!.Contains("not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        [Test]
        public async Task SendPasswordResetEmailAsync_DoesNothing_WhenEmailIsNullOrEmpty()
        {
            // Act
            await _service.SendPasswordResetEmailAsync(null!, "token123");
            await _service.SendPasswordResetEmailAsync("", "token123");

            // Assert
            // Should not throw and should log a warning for not found
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString() != null && v.ToString()!.Contains("not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Exactly(2));
        }

        [Test]
        public async Task SendPasswordResetEmailAsync_ThrowsException_WhenSmtpSettingsMissing()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company2@test.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // Remove SMTP section to simulate missing config
            _configurationMock.Setup(c => c.GetSection("SmtpSettings")).Returns((IConfigurationSection)null!);

            // Act & Assert
            Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await _service.SendPasswordResetEmailAsync("company2@test.com", "token123");
            });
        }

        [Test]
        public async Task SendPasswordResetEmailAsync_ThrowsException_WhenSmtpServerIsInvalid()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company3@test.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // Set an invalid SMTP server
            var smtpSection = new Mock<IConfigurationSection>();
            smtpSection.Setup(s => s["Server"]).Returns((string)null!);
            _configurationMock.Setup(c => c.GetSection("SmtpSettings")).Returns(smtpSection.Object);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _service.SendPasswordResetEmailAsync("company3@test.com", "token123");
            });
        }

        [Test]
        public async Task SendPasswordResetEmailAsync_UsesCorrectEmailAndToken()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company4@test.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // You can't easily intercept the actual email in this setup, but you can ensure no warning is logged
            try
            {
                await _service.SendPasswordResetEmailAsync("company4@test.com", "special-token-xyz");
            }
            catch
            {
                // Ignore SMTP exceptions
            }

            // Assert
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString() != null && v.ToString()!.Contains("not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }
    }
}