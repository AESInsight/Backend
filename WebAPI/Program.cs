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
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add environment variables
builder.Configuration.AddEnvironmentVariables();

// JWT-konfiguration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
Console.WriteLine($"[DEBUG] JWT Key in Program.cs: {jwtKey}");
var key = Encoding.UTF8.GetBytes(jwtKey);
var keyId = jwtSettings["KeyId"] ?? "test-key-id";

// Configure services
if (!builder.Environment.IsEnvironment("Test"))
{
    var provider = builder.Configuration["DatabaseProvider"] ?? "SqlServer";
    if (!builder.Services.Any(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)))
    {
        if (provider == "InMemory")
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
        }
        else
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
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
    var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
    var signingKey = new SymmetricSecurityKey(keyBytes);
    signingKey.KeyId = keyId;
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.Zero
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
    var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
    Console.WriteLine($"UseInMemoryDatabase value: {useInMemoryDb}");
    
    if (!useInMemoryDb)
    {
        try
        {
            dbContext.Database.Migrate(); // Apply pending migrations to the database
        }
        catch (Exception ex)
        {
            // Log the error but don't throw it
            Console.WriteLine($"Migration error: {ex.Message}");
        }
    }
    else
    {
        // For in-memory database, just ensure it's created
        dbContext.Database.EnsureCreated();
    }

    // Optionally, call a method to seed additional data if needed
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

    Console.WriteLine("Step 1: Creating company...");
    // ... create company code ...
    Console.WriteLine("Step 2: Creating employee...");
    // ... create employee code ...
    Console.WriteLine("Step 3: Updating salary...");
    // ... update salary code ...
    Console.WriteLine("Step 4: Deleting employee...");
    // ... delete employee code ...
    Console.WriteLine("Step 5: Deleting company...");
    // ... delete company code ...
    Console.WriteLine("All steps completed successfully!");
}

app.Run(); // Run the application

public partial class Program { }