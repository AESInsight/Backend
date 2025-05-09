using Backend.Data;
using Backend.Models;
using Backend.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Tests.Services
{
    [TestFixture]
    public class TestEmailService
    {
        private EmailService _emailService;
        private Mock<IConfiguration> _configurationMock;
        private Mock<ILogger<EmailService>> _loggerMock;
        private ApplicationDbContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            // Mock the IConfigurationSection for "SkipSmtp"
            var skipSmtpSectionMock = new Mock<IConfigurationSection>();
            skipSmtpSectionMock.Setup(x => x.Value).Returns("true");

            // Mock the IConfigurationSection for "SmtpSettings:Server"
            var smtpServerSectionMock = new Mock<IConfigurationSection>();
            smtpServerSectionMock.Setup(x => x.Value).Returns("smtp.example.com");

            // Mock the IConfiguration
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x.GetSection("SkipSmtp")).Returns(skipSmtpSectionMock.Object);
            _configurationMock.Setup(x => x.GetSection("SmtpSettings:Server")).Returns(smtpServerSectionMock.Object);

            _loggerMock = new Mock<ILogger<EmailService>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _dbContext.Companies.Add(new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Test Company",
                Email = "test@example.com",
                CVR = "12345678",
                Industry = "Tech",
                PasswordHash = Encoding.UTF8.GetBytes("hashedpassword1")
            });

            _dbContext.SaveChanges();

            _emailService = new EmailService(_configurationMock.Object, _loggerMock.Object, _dbContext);
        }

        [Test]
        public async Task SendPasswordResetEmailAsync_LogsWarning_WhenCompanyNotFound()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var resetToken = "test-reset-token";

            // Act
            await _emailService.SendPasswordResetEmailAsync(email, resetToken);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    #pragma warning disable CS8602
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Company with email nonexistent@example.com not found")),
                    #pragma warning restore CS8602
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task SendPasswordResetEmailAsync_SendsEmailSuccessfully()
        {
            // Arrange
            var email = "test@example.com";
            var resetToken = "test-reset-token";

            // Act
            try
            {
#pragma warning disable CS8602
                await _emailService.SendPasswordResetEmailAsync(email, resetToken);
#pragma warning restore CS8602
            }
            catch (Exception ex)
            {
                // Assert
                Assert.Fail($"Unexpected exception: {ex.Message}");
            }

            // Assert
            Assert.Pass("EmailService executed without throwing exceptions.");
        }
    }
}