using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Backend.Data;
using Backend.Models;
using Backend.Models.DTO;
using Backend.Services;
using NUnit.Framework;
using System.Text;

namespace WebAPI.Tests.Integration;

[TestFixture]
public class IntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private IServiceScope _scope;
    private ApplicationDbContext _dbContext;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove existing DbContext
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    // Add in-memory database
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });
    }

    [SetUp]
    public void Setup()
    {
        Console.WriteLine("Setting up test environment...");
        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _dbContext.Database.EnsureCreated();

        // Seed the database with required data
        SeedDatabase();
        Console.WriteLine("Database seeded successfully.");
    }

    private void SeedDatabase()
    {
        // Clear existing data
        _dbContext.Employee.RemoveRange(_dbContext.Employee);
        _dbContext.Companies.RemoveRange(_dbContext.Companies);
        _dbContext.Salaries.RemoveRange(_dbContext.Salaries);
        _dbContext.Users.RemoveRange(_dbContext.Users);
        _dbContext.SaveChanges();

        // Seed Companies
        _dbContext.Companies.Add(new CompanyModel
        {
            CompanyID = 1,
            CompanyName = "Test Company",
            Industry = "Technology",
            CVR = "12345678",
            Email = "test@company.com"
        });

        // Seed Employees
        _dbContext.Employee.Add(new EmployeeModel
        {
            EmployeeID = 1,
            JobTitle = "Test Developer",
            Experience = 3,
            Gender = "Female",
            CompanyID = 1
        });

        // Seed Salaries
        _dbContext.Salaries.Add(new SalaryModel
        {
            EmployeeID = 1,
            Salary = 50000.0,
            Timestamp = DateTime.Now
        });

        // Seed Users
        var adminHmac = new System.Security.Cryptography.HMACSHA512();
        var userHmac = new System.Security.Cryptography.HMACSHA512();

        _dbContext.Users.AddRange(
            new User
            {
                UserId = 1,
                Email = "admin",
                PasswordHash = adminHmac.ComputeHash(Encoding.UTF8.GetBytes("adminpassword")),
                PasswordSalt = adminHmac.Key,
                Role = "Admin"
            },
            new User
            {
                UserId = 2,
                Email = "user",
                PasswordHash = userHmac.ComputeHash(Encoding.UTF8.GetBytes("userpassword")),
                PasswordSalt = userHmac.Key,
                Role = "User"
            }
        );

        _dbContext.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _scope.Dispose();
        _client.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _factory.Dispose();
    }

    // Helper methods for common setup
    private async Task<CompanyDTO> CreateTestCompany()
    {
        var company = new CompanyDTO
        {
            CompanyName = "Test Company",
            Industry = "Technology",
            CVR = "12345678",
            Email = "test2@company.com"
        };

        var response = await _client.PostAsJsonAsync("/api/company", new List<CompanyDTO> { company }); // Send List<CompanyDTO>
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error response: {errorContent}");
            response.EnsureSuccessStatusCode();
        }

        var allCompanies = await _client.GetFromJsonAsync<List<CompanyDTO>>("/api/company");
        return allCompanies?.First() ?? throw new Exception("Failed to create test company");
    }

    private async Task<EmployeeDto> CreateTestEmployee(int companyId)
    {
        var employee = new EmployeeModel
        {
            JobTitle = "Test Developer",
            Experience = 3,
            Gender = "Female",
            CompanyID = companyId
        };

        var response = await _client.PostAsJsonAsync("/api/employee/add", new List<EmployeeModel> { employee });
        response.EnsureSuccessStatusCode();

        var allEmployees = await _client.GetFromJsonAsync<List<EmployeeDto>>("/api/employee/GetAllEmployees");
        return allEmployees?.First(e => e.JobTitle == employee.JobTitle) ?? throw new Exception("Failed to create test employee");
    }

    private async Task<SalaryDto> CreateTestSalary(int employeeId)
    {
        var salary = new SalaryModel
        {
            EmployeeID = employeeId,
            Salary = 50000.0,
            Timestamp = DateTime.Now
        };

        var response = await _client.PostAsJsonAsync("/api/salary/add", salary);
        response.EnsureSuccessStatusCode();

        var salaries = await _client.GetFromJsonAsync<List<SalaryDto>>($"/api/salary/employee/{employeeId}");
        return salaries?.First() ?? throw new Exception("Failed to create test salary");
    }

    // Employee Tests
    [Test]
    public async Task GetEmployees_ReturnsSuccessAndCorrectContentType()
    {
        Console.WriteLine("Testing GetEmployees endpoint...");
        var response = await _client.GetAsync("/api/employee/GetAllEmployees");
        response.EnsureSuccessStatusCode();
        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
    }

    [Test]
    public async Task GetEmployeeById_WithValidId_ReturnsEmployee()
    {
        var company = await CreateTestCompany();
        var employee = await CreateTestEmployee(company.CompanyID);

        var response = await _client.GetAsync($"/api/employee/{employee.EmployeeID}");
        response.EnsureSuccessStatusCode();

        var retrievedEmployee = await response.Content.ReadFromJsonAsync<EmployeeDto>();
        Assert.That(retrievedEmployee, Is.Not.Null);
        Assert.That(retrievedEmployee.JobTitle, Is.EqualTo(employee.JobTitle));
    }

    [Test]
    public async Task GetEmployeeById_WithInvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/employee/999999"); // Non-existent employee ID
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateEmployee_WithInvalidData_ReturnsBadRequest()
    {
        var employee = new EmployeeModel
        {
            JobTitle = "", // Invalid: empty job title
            Experience = -1, // Invalid: negative experience
            Gender = "Unknown", // Invalid: unsupported gender
            CompanyID = 999999 // Invalid: non-existent company ID
        };

        var response = await _client.PostAsJsonAsync("/api/employee/add", new List<EmployeeModel> { employee });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    // Company Tests
    [Test]
    public async Task CreateCompany_WithValidData_ReturnsSuccess()
    {
        var company = await CreateTestCompany();

        Assert.That(company, Is.Not.Null);
        Assert.That(company.CompanyName, Is.EqualTo("Test Company"));
    }

    [Test]
    public async Task CreateCompany_WithInvalidData_ReturnsBadRequest()
    {
        var company = new CompanyDTO
        {
            CompanyName = "", // Invalid: empty name
            Industry = "Technology",
            CVR = "123", // Invalid: too short
            Email = "invalid-email" // Invalid: wrong format
        };

        var response = await _client.PostAsJsonAsync("/api/company", new List<CompanyDTO> { company });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetCompanyById_WithValidId_ReturnsCompany()
    {
        var company = await CreateTestCompany();

        var response = await _client.GetAsync($"/api/company/{company.CompanyID}");
        response.EnsureSuccessStatusCode();

        var retrievedCompany = await response.Content.ReadFromJsonAsync<CompanyDTO>();
        Assert.That(retrievedCompany, Is.Not.Null);
        Assert.That(retrievedCompany.CompanyName, Is.EqualTo(company.CompanyName));
    }

    [Test]
    public async Task GetCompanyById_WithInvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/company/999999"); // Non-existent company ID
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    // Salary Tests
    [Test]
    public async Task CreateSalary_WithValidData_ReturnsSuccess()
    {
        var company = await CreateTestCompany();
        var employee = await CreateTestEmployee(company.CompanyID);
        var salary = await CreateTestSalary(employee.EmployeeID);

        Assert.That(salary, Is.Not.Null);
        Assert.That(salary.Salary, Is.EqualTo(50000.0).Within(0.01));
    }

    [Test]
    public async Task CreateSalary_WithInvalidData_ReturnsBadRequest()
    {
        var salary = new SalaryModel
        {
            EmployeeID = 999999, // Non-existent employee
            Salary = -1000.0, // Invalid: negative amount
            Timestamp = DateTime.Now
        };

        var response = await _client.PostAsJsonAsync("/api/salary/add", salary);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetSalariesByEmployeeId_WithValidId_ReturnsSalaries()
    {
        var company = await CreateTestCompany();
        var employee = await CreateTestEmployee(company.CompanyID);
        await CreateTestSalary(employee.EmployeeID);

        var response = await _client.GetAsync($"/api/salary/employee/{employee.EmployeeID}");
        response.EnsureSuccessStatusCode();

        var salaries = await response.Content.ReadFromJsonAsync<List<SalaryDto>>();
        Assert.That(salaries, Is.Not.Null);
        Assert.That(salaries.Count, Is.GreaterThan(0));
    }

    [Test]
public async Task GetSalariesByEmployeeId_WithInvalidId_ReturnsNotFound()
{
    var response = await _client.GetAsync("/api/salary/employee/999999");
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
}


    [Test]
    public async Task UpdateSalary_WithValidData_ReturnsSuccess()
    {
        var company = await CreateTestCompany();
        var employee = await CreateTestEmployee(company.CompanyID);
        var salary = await CreateTestSalary(employee.EmployeeID);

        var updatedSalary = new SalaryModel
        {
            EmployeeID = employee.EmployeeID,
            Salary = 60000.0, // Updated salary
            Timestamp = DateTime.Now
        };

        var response = await _client.PostAsJsonAsync("/api/salary/add", updatedSalary);
        response.EnsureSuccessStatusCode();

        var salaries = await _client.GetFromJsonAsync<List<SalaryDto>>($"/api/salary/employee/{employee.EmployeeID}");
        Assert.That(salaries, Is.Not.Null);
        Assert.That(salaries.Any(s => s.Salary == 60000.0), Is.True);
    }

    [Test]
    public async Task UpdateSalary_WithInvalidData_ReturnsBadRequest()
    {
        var salary = new SalaryModel
        {
            EmployeeID = 999999, // Non-existent employee ID
            Salary = -5000.0, // Invalid: negative salary
            Timestamp = DateTime.Now
        };

        var response = await _client.PostAsJsonAsync("/api/salary/add", salary);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}