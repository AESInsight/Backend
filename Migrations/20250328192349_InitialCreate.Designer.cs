﻿// <auto-generated />
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250328192349_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Backend.Models.CompanyModel", b =>
                {
                    b.Property<int>("CompanyID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CVR")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("varchar(8)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("CompanyID");

                    b.ToTable("Company", (string)null);
                });

            modelBuilder.Entity("Backend.Models.EmployeeModel", b =>
                {
                    b.Property<string>("EmployeeID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("CompanyID")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("JobTitle")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<double>("Salary")
                        .HasColumnType("double");

                    b.HasKey("EmployeeID");

                    b.ToTable("Employee", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
