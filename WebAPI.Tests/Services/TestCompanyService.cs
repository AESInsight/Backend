using Backend.Data;
using Backend.Models;
using Backend.Models.DTO;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Tests.Services
{
    [TestFixture]
    public class TestCompanyService
    {
        private Mock<ApplicationDbContext> _dbContextMock;
        private Mock<ILogger<CompanyService>> _loggerMock;
        private CompanyService _companyService;

        [SetUp]
        public void SetUp()
        {
            _dbContextMock = new Mock<ApplicationDbContext>();
            _loggerMock = new Mock<ILogger<CompanyService>>();
            _companyService = new CompanyService(_dbContextMock.Object, _loggerMock.Object);
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return dbSetMock;
        }

        private Mock<DbSet<CompanyModel>> CreateMockDbSetForSingleEntity(CompanyModel? entity)
        {
            var dbSetMock = new Mock<DbSet<CompanyModel>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<int>())).ReturnsAsync(entity);
            return dbSetMock;
        }

        [Test]
        public async Task GetAllCompaniesAsync_ReturnsAllCompanies()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel { CompanyID = 1, CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com", PasswordHash = "Password123" },
                new CompanyModel { CompanyID = 2, CompanyName = "Company B", Industry = "Finance", CVR = "87654321", Email = "b@example.com", PasswordHash = "Password123" }
            }.AsQueryable();

            var dbSetMock = CreateMockDbSet(companies);
            _dbContextMock.Setup(db => db.Companies).Returns(dbSetMock.Object);

            // Act
            var result = await _companyService.GetAllCompaniesAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].CompanyName, Is.EqualTo("Company A"));
            Assert.That(result[1].CompanyName, Is.EqualTo("Company B"));
        }

        [Test]
        public async Task GetCompanyByIdAsync_ValidId_ReturnsCompany()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com",
                PasswordHash = "Password123"
            };

            var dbSetMock = CreateMockDbSetForSingleEntity(company);
            _dbContextMock.Setup(db => db.Companies).Returns(dbSetMock.Object);

            // Act
            var result = await _companyService.GetCompanyByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CompanyName, Is.EqualTo("Company A"));
        }

        [Test]
        public void GetCompanyByIdAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var dbSetMock = CreateMockDbSetForSingleEntity(null);
            _dbContextMock.Setup(db => db.Companies).Returns(dbSetMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _companyService.GetCompanyByIdAsync(999));
        }

        [Test]
        public async Task DeleteCompanyAsync_ValidId_DeletesCompany()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com",
                PasswordHash = "Password123"
            };

            var dbSetMock = CreateMockDbSetForSingleEntity(company);
            dbSetMock.Setup(m => m.Remove(company));

            _dbContextMock.Setup(db => db.Companies).Returns(dbSetMock.Object);
            _dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _companyService.DeleteCompanyAsync(1);

            // Assert
            dbSetMock.Verify(m => m.Remove(company), Times.Once);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public void DeleteCompanyAsync_InvalidId_DoesNotThrow()
        {
            // Arrange
            var dbSetMock = CreateMockDbSetForSingleEntity(null);
            _dbContextMock.Setup(db => db.Companies).Returns(dbSetMock.Object);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await _companyService.DeleteCompanyAsync(999));
        }

        [Test]
        public async Task CreateCompaniesAsync_ValidCompanies_AddsCompanies()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel { CompanyName = "Company A", CVR = "12345678", Email = "a@example.com", PasswordHash = "Password123", Industry = "Tech" },
                new CompanyModel { CompanyName = "Company B", CVR = "87654321", Email = "b@example.com", PasswordHash = "Password123", Industry = "Finance" }
            };

            var dbSetMock = new Mock<DbSet<CompanyModel>>();
            _dbContextMock.Setup(db => db.Companies).Returns(dbSetMock.Object);
            _dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _companyService.CreateCompaniesAsync(companies);

            // Assert
            dbSetMock.Verify(m => m.Add(It.IsAny<CompanyModel>()), Times.Exactly(2));
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task UpdateCompanyAsync_ValidId_UpdatesCompany()
        {
            // Arrange
            var existingCompany = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Old Company Name",
                Industry = "Tech",
                CVR = "12345678",
                Email = "old@example.com",
                PasswordHash = "OldPassword123"
            };

            var updatedCompany = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Updated Company Name",
                Industry = "Finance",
                CVR = "12345678",
                Email = "updated@example.com",
                PasswordHash = "UpdatedPassword123"
            };

            var dbSetMock = CreateMockDbSetForSingleEntity(existingCompany);
            _dbContextMock.Setup(db => db.Companies).Returns(dbSetMock.Object);
            _dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _companyService.UpdateCompanyAsync(updatedCompany);

            // Assert
            Assert.That(existingCompany.CompanyName, Is.EqualTo("Updated Company Name"));
            Assert.That(existingCompany.Industry, Is.EqualTo("Finance"));
            Assert.That(existingCompany.Email, Is.EqualTo("updated@example.com"));
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public void UpdateCompanyAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var updatedCompany = new CompanyModel
            {
                CompanyID = 999, // Non-existent ID
                CompanyName = "Updated Company Name",
                Industry = "Finance",
                CVR = "12345678",
                Email = "updated@example.com",
                PasswordHash = "UpdatedPassword123"
            };

            var dbSetMock = CreateMockDbSetForSingleEntity(null); // No company found
            _dbContextMock.Setup(db => db.Companies).Returns(dbSetMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _companyService.UpdateCompanyAsync(updatedCompany));
        }
    }
}