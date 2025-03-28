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
            // Removed the check for Development environment
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

                endpoints.MapGet("/api/employees", async (ApplicationDbContext dbContext) =>
                {
                    try
                    {
                        var employees = await dbContext.Employee.ToListAsync();
                        return Results.Ok(employees);
                    }
                    catch (Exception ex)
                    {
                        return Results.BadRequest(new { Status = "Error", Message = $"Failed to retrieve data: {ex.Message}" });
                    }
                });

                endpoints.MapGet("/api/employees/{id}", async (string id, ApplicationDbContext dbContext) =>
                {
                    try
                    {
                        var employee = await dbContext.Employee.FirstOrDefaultAsync(e => e.EmployeeID == id);
                        if (employee == null)
                        {
                            return Results.NotFound(new { Status = "NotFound", Message = $"Employee with ID {id} not found." });
                        }
                        return Results.Ok(employee);
                    }
                    catch (Exception ex)
                    {
                        return Results.BadRequest(new { Status = "Error", Message = $"Failed to retrieve data: {ex.Message}" });
                    }
                });
            });

            return app;
        }
    }
} 