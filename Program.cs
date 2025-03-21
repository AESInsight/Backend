using Microsoft.EntityFrameworkCore;
using Backend.Config;
using Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// Add database configuration
var dbConfig = builder.Configuration.GetSection("DatabaseConfig").Get<DatabaseConfig>();
// Override connection string from environment variable if available
var connectionString = Environment.GetEnvironmentVariable("GH_SECRET_CONNECTIONSTRING") 
    ?? dbConfig?.ConnectionString 
    ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));
    options.UseMySql(connectionString, serverVersion, mySqlOptions =>
    {
        mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        mySqlOptions.MaxPoolSize(dbConfig?.MaxPoolSize ?? 100);
        mySqlOptions.MinimumPoolSize(dbConfig?.MinPoolSize ?? 5);
        mySqlOptions.ConnectionLifetime(TimeSpan.FromSeconds(dbConfig?.ConnectionLifetime ?? 300));
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// Add database connection test endpoint
app.MapGet("/api/database/test", async (ApplicationDbContext dbContext) =>
{
    try
    {
        // Test the connection
        bool canConnect = await dbContext.Database.CanConnectAsync();
        
        if (canConnect)
        {
            return Results.Ok(new { Status = "Connected", Message = "Successfully connected to the database!" });
        }
        else
        {
            return Results.BadRequest(new { Status = "Failed", Message = "Could not connect to the database." });
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Status = "Error", Message = $"Connection test failed: {ex.Message}" });
    }
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
