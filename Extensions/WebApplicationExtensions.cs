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

                // Endpoint to retrieve all employees
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

                // Endpoint to retrieve an employee by ID
                endpoints.MapGet("/api/employees/{id}", async (string id, ApplicationDbContext dbContext) =>
                {
                    try
                    {
                        // Convert id from string to int
                        if (!int.TryParse(id, out int employeeId))
                        {
                            return Results.BadRequest(new { Status = "Error", Message = "Invalid ID format. ID must be an integer." });
                        }

                        // Retrieve employee based on ID
                        var employee = await dbContext.Employee.FirstOrDefaultAsync(e => e.EmployeeID == employeeId);
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