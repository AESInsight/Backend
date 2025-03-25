using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<YourModel> YourModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply seed data
        DbSeeder.SeedData(modelBuilder);
    }

    // Add your DbSet properties here for your entities
    // Example:
    // public DbSet<YourEntity> YourEntities { get; set; }
} 