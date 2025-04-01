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

    public DbSet<User> Users { get; set; }

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
        // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    Password = "$2a$11$K7tih2DcSVCdM9LTf5lmne41uEffe6LXZHT7AmV4mGc4/vbB1NIiG", // Hashed password
                    Role = "Admin"
                },
                new User
                {
                    UserId = 2,
                    Username = "user",
                    Password = "$2a$11$ymlFcuiFXHLSXD3yEvFp5uFrvkf8FIT6wn/nQYDYgb7B.O0T4oTYS", // Hashed password
                    Role = "User"
                }
            );
    }
}