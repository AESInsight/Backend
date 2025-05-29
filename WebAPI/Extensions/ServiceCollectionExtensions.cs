using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Config;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace Backend.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("GH_SECRET_CONNECTIONSTRING") 
                ?? throw new InvalidOperationException("GitHub secret connection string not found");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mysqlOptions => {
                        mysqlOptions.CommandTimeout(120); // Increase timeout to 120 seconds
                    }
                ));

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder =>
                    {
                    builder.WithOrigins(
                            "http://localhost:5173", // Frontend URL for local development
                            "https://aes-insight.dk" // Production website
                        )
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            return services;
        }
    }
} 