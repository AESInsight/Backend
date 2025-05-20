using Microsoft.EntityFrameworkCore;
using Backend.Models;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Backend.Data;

[ExcludeFromCodeCoverage]
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

    // DbSet for SalaryModel
    public DbSet<SalaryModel> Salaries { get; set; }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names explicitly
        modelBuilder.Entity<EmployeeModel>().ToTable("Employee");
        modelBuilder.Entity<CompanyModel>().ToTable("Company");

        // Configure the relationship between EmployeeModel and CompanyModel
        modelBuilder.Entity<EmployeeModel>()
            .HasOne(e => e.Company)
            .WithMany()
            .HasForeignKey(e => e.CompanyID)
            .OnDelete(DeleteBehavior.Cascade);

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

        // Seed Users
        var adminHmac = new System.Security.Cryptography.HMACSHA512();
        var userHmac = new System.Security.Cryptography.HMACSHA512();

        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                Email = "admin",
                PasswordHash = adminHmac.ComputeHash(Encoding.UTF8.GetBytes("adminpassword")),
                PasswordSalt = adminHmac.Key,
                Role = "Admin"
            },
            new User
            {
                UserId = 2,
                Email = "user",
                PasswordHash = userHmac.ComputeHash(Encoding.UTF8.GetBytes("userpassword")),
                PasswordSalt = userHmac.Key,
                Role = "User"
            }
        );
    }
}