using Microsoft.EntityFrameworkCore;
using Backend.Config;
using Backend.Data;
using Backend.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.ConfigureCors();

var app = builder.Build();

// Configure middleware
app.UseSwaggerIfDevelopment();
app.UseHttpsRedirection();
app.UseCorsPolicy();

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