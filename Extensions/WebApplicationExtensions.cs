using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Backend.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Extensions
{
    public static class WebApplicationExtensions
    {
        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", builder =>
                {
                    builder.WithOrigins("http://localhost:5174") // Frontend URL
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerIfDevelopment(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetService<IWebHostEnvironment>();
            // Enable Swagger and Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }

        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {
            // Apply CORS policy named "AllowFrontend"
            app.UseCors("AllowFrontend");
            return app;
        }

        public static IApplicationBuilder UseRoutingAndEndpoints(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Map controller routes
            });
            return app;
        }
    }
}