using Microsoft.EntityFrameworkCore;
using Backend.Config;
using Backend.Data;
using Backend.Extensions;
using Backend.Services;
using System.Text; // For Encoding
using Microsoft.AspNetCore.Authentication.JwtBearer; // For JwtBearerDefaults
using Microsoft.IdentityModel.Tokens; // For TokenValidationParameters and SymmetricSecurityKey
using Microsoft.OpenApi.Models; // For OpenApiInfo, OpenApiSecurityScheme, etc.
using Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add environment variables
builder.Configuration.AddEnvironmentVariables();

// JWT-konfiguration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
var key = Encoding.UTF8.GetBytes(jwtKey);

// Configure services
builder.Services.ConfigureDatabase(builder.Configuration); // Configure the database connection
builder.Services.ConfigureSwagger(); // Configure Swagger for API documentation
builder.Services.ConfigureCors(); // Explicitly use the custom ConfigureCors method
builder.Services.AddControllers(); // Add support for controllers

// Register services
builder.Services.AddScoped<IEmployeeService, EmployeeService>(); // Register EmployeeService
builder.Services.AddScoped<ICompanyService, CompanyService>(); // Register CompanyService
builder.Services.AddScoped<IEmailService, EmailService>(); // Register EmailService
builder.Services.AddScoped<ISalaryService, SalaryService>(); // Register SalaryService

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Booking API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token. Example: 'Bearer abc123'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured"),
        ValidAudience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured"),
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware
app.UseSwaggerDevelop(); // Enable Swagger in development environment
app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseCorsPolicy(); // Apply the configured CORS policy
app.UseRouting(); // Enable routing
app.UseAuthorization(); // Enable authorization
app.MapControllers(); // Map controller routes

// Apply migrations and seed the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    if (env != "Testing")
    {
        dbContext.Database.Migrate(); // Apply pending migrations to the database
    }

    // Query company with ID 4
    var company = await dbContext.Companies.FindAsync(4);
    if (company != null)
    {
        Console.WriteLine($"Company ID: {company.CompanyID}");
        Console.WriteLine($"Company Name: {company.CompanyName}");
        Console.WriteLine($"Email: {company.Email}");
    }
    else
    {
        Console.WriteLine("Company with ID 4 not found");
    }
}

app.Run(); // Run the application


public partial class Program { }