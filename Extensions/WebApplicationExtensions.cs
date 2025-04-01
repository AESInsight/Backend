using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Backend.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Extensions
{
    public static class WebApplicationExtensions
    {
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
                // Endpoint to test database connection
                endpoints.MapGet("/api/database/test", async (ApplicationDbContext dbContext) =>
                {
                    try
                    {
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
            });

            return app;
        }
    }
}