using Backend.Controllers;
using Backend.Models;
using Backend.Models.DTO;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Tests.Controllers
{
    [TestFixture]
    public class TestCompanyController
    {
        private Mock<ICompanyService> _companyServiceMock;
        private CompanyController _controller;

        [SetUp]
        public void SetUp()
        {
            _companyServiceMock = new Mock<ICompanyService>();
            _controller = new CompanyController(_companyServiceMock.Object);
        }

        [Test]
        public async Task GetAllCompanies_ReturnsOkResultWithCompanies()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel
                {
                    CompanyID = 1,
                    CompanyName = "Company A",
                    Industry = "Tech",
                    CVR = "12345678",
                    Email = "a@example.com",
                    PasswordHash = Encoding.UTF8.GetBytes("hash1") // Convert string to byte[]
                },
                new CompanyModel
                {
                    CompanyID = 2,
                    CompanyName = "Company B",
                    Industry = "Finance",
                    CVR = "87654321",
                    Email = "b@example.com",
                    PasswordHash = Encoding.UTF8.GetBytes("hash2") // Convert string to byte[]
                }
            };
            _companyServiceMock.Setup(s => s.GetAllCompaniesAsync()).ReturnsAsync(companies);

            // Act
            var result = await _controller.GetAllCompanies() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var companyDTOs = result.Value as List<CompanyDTO>;
            Assert.That(companyDTOs, Is.Not.Null);
            Assert.That(companyDTOs.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task InsertCompanies_ReturnsOkResult()
        {
            // Arrange
            var companyDTOs = new List<CompanyDTO>
            {
                new CompanyDTO { CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com" }
            };

            // Mock the CreateCompaniesAsync method to simulate successful insertion
            _companyServiceMock
                .Setup(s => s.CreateCompaniesAsync(It.IsAny<List<CompanyModel>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.InsertCompanies(companyDTOs) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            // Cast the result.Value to a dictionary
            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Companies inserted successfully"));
            Assert.That(response["count"], Is.EqualTo(1)); // Ensure the count matches the number of companies inserted
        }

        [Test]
        public async Task DeleteCompany_ReturnsNoContentResult()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ReturnsAsync(new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com",
                PasswordHash = Encoding.UTF8.GetBytes("password1")
            });

            // Act
            var result = await _controller.DeleteCompany(1) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public async Task DeleteCompany_ReturnsNotFoundResult()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ThrowsAsync(new KeyNotFoundException("Company not found"));

            // Act
            var result = await _controller.DeleteCompany(1) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("Company not found"));
        }

        [Test]
        public async Task GetCompanyById_ReturnsOkResultWithCompany()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com",
                PasswordHash = Encoding.UTF8.GetBytes("password1")
            };

            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ReturnsAsync(company);

            // Act
            var result = await _controller.GetCompanyById(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var companyDTO = result.Value as CompanyDTO;
            Assert.That(companyDTO, Is.Not.Null);
            Assert.That(companyDTO.CompanyName, Is.EqualTo("Company A"));
        }

        [Test]
        public async Task GetCompanyById_ReturnsNotFoundResult()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ThrowsAsync(new KeyNotFoundException("Company not found"));

            // Act
            var result = await _controller.GetCompanyById(1) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("Company not found"));
        }

        [Test]
        public async Task UpdateCompany_ReturnsNoContentResult()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Updated Company",
                Industry = "Tech",
                CVR = "12345678",
                Email = "updated@example.com",
                PasswordHash = Encoding.UTF8.GetBytes("updatedhash")
            };

            _companyServiceMock.Setup(s => s.UpdateCompanyAsync(company)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCompany(1, company) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public async Task UpdateCompany_ReturnsBadRequestResult()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 2, // Mismatched ID
                CompanyName = "Updated Company",
                Industry = "Tech",
                CVR = "12345678",
                Email = "updated@example.com",
                PasswordHash = Encoding.UTF8.GetBytes("updatedhash")
            };

            // Act
            var result = await _controller.UpdateCompany(1, company) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Invalid company data"));
        }

        [Test]
        public async Task GenerateSampleCompanies_ReturnsOkResult()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GenerateSampleCompaniesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.GenerateSampleCompanies() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Sample companies generated successfully"));
        }

        [Test]
        public async Task DeleteAllCompanies_ReturnsOkResult()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.DeleteAllCompaniesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAllCompanies() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("All companies deleted successfully"));
        }

        [Test]
        public async Task GetAllIndustries_ReturnsOkResultWithIndustries()
        {
            // Arrange
            var industries = new List<string> { "Tech", "Finance", "Healthcare" };
            _companyServiceMock.Setup(s => s.GetAllIndustriesAsync()).ReturnsAsync(industries);

            // Act
            var result = await _controller.GetAllIndustries() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var industryList = result.Value as List<string>;
            Assert.That(industryList, Is.Not.Null);
            Assert.That(industryList.Count, Is.EqualTo(3));
            Assert.That(industryList, Is.EquivalentTo(industries));
        }
    }
}