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

        // Konfigurer tabelnavne eksplicit
        modelBuilder.Entity<EmployeeModel>().ToTable("Employee");
        modelBuilder.Entity<CompanyModel>().ToTable("Company");

        // Ekstra konfiguration for CompanyModel
        modelBuilder.Entity<CompanyModel>(entity =>
        {
            entity.HasKey(c => c.CompanyID); // Primærnøgle
            entity.Property(c => c.CompanyName)
                  .IsRequired()
                  .HasMaxLength(255); // Maks. længde for CompanyName
            entity.Property(c => c.CVR)
                  .IsRequired()
                  .HasMaxLength(8); // CVR skal være præcis 8 tegn
        });
    }
}