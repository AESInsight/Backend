using Microsoft.EntityFrameworkCore;
using Backend.Config;
using Backend.Data;
using Backend.Extensions;
using Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add environment variables
builder.Configuration.AddEnvironmentVariables();

// Configure services
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.ConfigureCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Register services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

var app = builder.Build();

// Configure middleware
app.UseSwaggerIfDevelopment();
app.UseHttpsRedirection();
app.UseCorsPolicy();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Configure routing and endpoints
app.UseRoutingAndEndpoints();

// Apply migrations and seed the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    // Optionally, call a method to seed additional data if needed
}

app.Run();