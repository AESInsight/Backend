using Microsoft.EntityFrameworkCore;
using Backend.Models;


namespace Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<EmployeeModel> Employee { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the table name explicitly
        modelBuilder.Entity<EmployeeModel>().ToTable("Employee");
    }

    // Add your DbSet properties here for your entities
    // Example:
    // public DbSet<YourEntity> YourEntities { get; set; }
} 