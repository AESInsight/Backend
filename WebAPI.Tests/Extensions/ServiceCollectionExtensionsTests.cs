using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Backend.Extensions;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

public class ServiceCollectionExtensionsTests
{
    [Test]
    public void ConfigureDatabase_RegistersDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var inMemorySettings = new Dictionary<string, string>();
        Environment.SetEnvironmentVariable("GH_SECRET_CONNECTIONSTRING", "server=localhost;database=testdb;user=root;password=pass;");
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        services.ConfigureDatabase(configuration);
        var provider = services.BuildServiceProvider();
        var dbContext = provider.GetService<ApplicationDbContext>();

        // Assert
        Assert.NotNull(dbContext);
        Assert.True(dbContext.Database.IsMySql());
    }

    [Fact]
    public void ConfigureCors_RegistersPolicy()
    {
        var services = new ServiceCollection();

        services.ConfigureCors();
        var provider = services.BuildServiceProvider();
        var corsOptions = provider.GetRequiredService<IOptions<CorsOptions>>();

        Assert.True(corsOptions.Value.GetPolicy("AllowFrontend") != null);
    }

    [Fact]
    public void ConfigureSwagger_RegistersSwaggerGen()
    {
        var services = new ServiceCollection();

        services.ConfigureSwagger();
        var provider = services.BuildServiceProvider();

        var swaggerGen = provider.GetService<Microsoft.Extensions.Options.IOptions<Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions>>();
        Assert.NotNull(swaggerGen);
    }

}
