using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet for EmployeeModel
    public DbSet<EmployeeModel> Employee { get; set; }

    // DbSet for CompanyModel
    public DbSet<CompanyModel> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names explicitly
        modelBuilder.Entity<EmployeeModel>().ToTable("Employee");
        modelBuilder.Entity<CompanyModel>().ToTable("Company");

        // Additional configuration for CompanyModel
        modelBuilder.Entity<CompanyModel>(entity =>
        {
            entity.HasKey(c => c.CompanyID); // Primary key
            entity.Property(c => c.CompanyName)
                  .IsRequired()
                  .HasMaxLength(255); // Maximum length for CompanyName
            entity.Property(c => c.CVR)
                  .IsRequired()
                  .HasMaxLength(8); // CVR must be exactly 8 characters
        });
    }
}