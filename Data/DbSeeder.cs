using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public static class DbSeeder
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        // Add your seed data here
        // Example:
        modelBuilder.Entity<YourModel>().HasData(
            new YourModel
            {
                Id = 1,
                Name = "Test Item 1",
                Description = "This is a new test item"
            },
            new YourModel
            {
                Id = 2,
                Name = "Test Item 2",
                Description = "This is another newer test item"
            }
        );
    }
} 