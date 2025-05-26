using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Backend.Data;
using Backend.Models;
using Backend.Models.DTO;
using Backend.Services;
using Xunit;

namespace WebAPI.Tests.Integration;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        var dbName = Guid.NewGuid().ToString();
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                // Add in-memory database
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(dbName));

                // Remove and re-register IEmployeeService
                var serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmployeeService));
                if (serviceDescriptor != null) services.Remove(serviceDescriptor);
                services.AddScoped<IEmployeeService, EmployeeService>();

                // Ensure database is created
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    // Helper methods for common setup
    private async Task<CompanyDTO> CreateTestCompany()
    {
        var company = new CompanyDTO
        {
            CompanyName = "Test Company",
            Industry = "Technology",
            CVR = "12345678",
            Email = "test@company.com"
        };
        var response = await _client.PostAsJsonAsync("/api/company", new List<CompanyDTO> { company });
        response.EnsureSuccessStatusCode();
        
        var allCompanies = await _client.GetFromJsonAsync<List<CompanyDTO>>("/api/company");
        return allCompanies.First();
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
        return allEmployees.First(e => e.JobTitle == employee.JobTitle);
    }

    private async Task<SalaryDTO> CreateTestSalary(int employeeId)
    {
        var salary = new SalaryModel
        {
            EmployeeID = employeeId,
            Salary = 50000.0,
            Timestamp = DateTime.Now
        };
        var response = await _client.PostAsJsonAsync("/api/salary/add", salary);
        response.EnsureSuccessStatusCode();
        
        var salaries = await _client.GetFromJsonAsync<List<SalaryDTO>>($"/api/salary/employee/{employeeId}");
        return salaries.First();
    }

    // Employee Tests
    [Fact]
    public async Task GetEmployees_ReturnsSuccessAndCorrectContentType()
    {
        var company = await CreateTestCompany();
        await CreateTestEmployee(company.CompanyID);

        var response = await _client.GetAsync("/api/employee/GetAllEmployees");
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetEmployeeById_WithValidId_ReturnsEmployee()
    {
        var company = await CreateTestCompany();
        var employee = await CreateTestEmployee(company.CompanyID);

        var response = await _client.GetAsync($"/api/employee/{employee.EmployeeID}");
        response.EnsureSuccessStatusCode();
        
        var retrievedEmployee = await response.Content.ReadFromJsonAsync<EmployeeDto>();
        Assert.NotNull(retrievedEmployee);
        Assert.Equal(employee.JobTitle, retrievedEmployee.JobTitle);
    }

    // Company Tests
    [Fact]
    public async Task CreateCompany_WithValidData_ReturnsSuccess()
    {
        var company = await CreateTestCompany();
        Assert.NotNull(company);
        Assert.Equal("Test Company", company.CompanyName);
    }

    [Fact]
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
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Salary Tests
    [Fact]
    public async Task CreateSalary_WithValidData_ReturnsSuccess()
    {
        var company = await CreateTestCompany();
        var employee = await CreateTestEmployee(company.CompanyID);
        var salary = await CreateTestSalary(employee.EmployeeID);

        Assert.NotNull(salary);
        Assert.Equal(50000.0m, salary.Salary);
    }

    [Fact]
    public async Task CreateSalary_WithInvalidData_ReturnsBadRequest()
    {
        var salary = new SalaryModel
        {
            EmployeeID = 999999, // Non-existent employee
            Salary = -1000.0, // Invalid: negative amount
            Timestamp = DateTime.Now
        };

        var response = await _client.PostAsJsonAsync("/api/salary/add", salary);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
} 