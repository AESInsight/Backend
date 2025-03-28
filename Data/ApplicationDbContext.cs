using Microsoft.EntityFrameworkCore;
using Backend.Models;


namespace Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<EmployeeModel> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    // Add your DbSet properties here for your entities
    // Example:
    // public DbSet<YourEntity> YourEntities { get; set; }
} 