﻿// <auto-generated />
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250408091727_AddEmailPasswordField")]
    partial class AddEmailPasswordField
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Backend.Models.CompanyModel", b =>
                {
                    b.Property<int>("CompanyID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("CompanyID"));

                    b.Property<string>("CVR")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("varchar(8)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EmailPassword")
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("CompanyID");

                    b.ToTable("Company", (string)null);
                });

            modelBuilder.Entity("Backend.Models.EmployeeModel", b =>
                {
                    b.Property<int>("EmployeeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("EmployeeID"));

                    b.Property<int>("CompanyID")
                        .HasColumnType("int");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<string>("Gender")
                        .HasColumnType("longtext");

                    b.Property<string>("JobTitle")
                        .HasColumnType("longtext");

                    b.Property<double>("Salary")
                        .HasColumnType("double");

                    b.HasKey("EmployeeID");

                    b.HasIndex("CompanyID");

                    b.ToTable("Employee", (string)null);
                });

            modelBuilder.Entity("Backend.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            Password = "$2a$11$K7tih2DcSVCdM9LTf5lmne41uEffe6LXZHT7AmV4mGc4/vbB1NIiG",
                            Role = "Admin",
                            Username = "admin"
                        },
                        new
                        {
                            UserId = 2,
                            Password = "$2a$11$ymlFcuiFXHLSXD3yEvFp5uFrvkf8FIT6wn/nQYDYgb7B.O0T4oTYS",
                            Role = "User",
                            Username = "user"
                        });
                });

            modelBuilder.Entity("Backend.Models.EmployeeModel", b =>
                {
                    b.HasOne("Backend.Models.CompanyModel", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });
#pragma warning restore 612, 618
        }
    }
}
