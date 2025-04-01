using Microsoft.EntityFrameworkCore;
using Backend.Config;
using Backend.Data;
using Backend.Extensions;
using Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add environment variables
builder.Configuration.AddEnvironmentVariables();

// Configure services
builder.Services.ConfigureDatabase(builder.Configuration); // Configure the database connection
builder.Services.ConfigureSwagger(); // Configure Swagger for API documentation
builder.Services.ConfigureCors(); // Configure CORS policy
builder.Services.AddControllers(); // Add support for controllers
builder.Services.AddEndpointsApiExplorer(); // Add support for endpoint exploration
builder.Services.AddSwaggerGen(); // Add Swagger generation

// Register services
builder.Services.AddScoped<IEmployeeService, EmployeeService>(); // Register EmployeeService
builder.Services.AddScoped<ICompanyService, CompanyService>(); // Register CompanyService

var app = builder.Build();

// Configure middleware
app.UseSwaggerIfDevelopment(); // Enable Swagger in development environment
app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseCorsPolicy(); // Apply the configured CORS policy
app.UseRouting(); // Enable routing
app.UseAuthorization(); // Enable authorization
app.MapControllers(); // Map controller routes

// Configure routing and endpoints
app.UseRoutingAndEndpoints(); // Add custom routing and endpoints

// Apply migrations and seed the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // Apply pending migrations to the database
    // Optionally, call a method to seed additional data if needed
}

app.Run(); // Run the application