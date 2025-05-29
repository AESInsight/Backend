using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Backend.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Backend.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class WebApplicationExtensions
    {
        public static IApplicationBuilder UseSwaggerDevelop(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetService<IWebHostEnvironment>();
            // Enable Swagger and Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }

        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors("AllowFrontend");
            return app;
        }

        public static IApplicationBuilder UseRoutingAndEndpoints(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            return app;
        }
    }
}