﻿// <auto-generated />
using System;
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
    [Migration("20250506122127_RenameUsernameToEmail_RemoveEmployeeModelEmployeeID_RemoveSalaryIDFromEmployee")]
    partial class RenameUsernameToEmail_RemoveEmployeeModelEmployeeID_RemoveSalaryIDFromEmployee
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

                    b.Property<string>("Industry")
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

                    b.HasKey("EmployeeID");

                    b.HasIndex("CompanyID");

                    b.ToTable("Employee", (string)null);
                });

            modelBuilder.Entity("Backend.Models.SalaryModel", b =>
                {
                    b.Property<int>("SalaryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("SalaryID"));

                    b.Property<int>("EmployeeID")
                        .HasColumnType("int");

                    b.Property<double>("Salary")
                        .HasColumnType("double");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("SalaryID");

                    b.ToTable("Salaries");
                });

            modelBuilder.Entity("Backend.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("UserId"));

                    b.Property<int?>("CompanyID")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<string>("ResetPasswordToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("ResetPasswordTokenExpiry")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.HasKey("UserId");

                    b.HasIndex("CompanyID");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            Email = "admin",
                            PasswordHash = new byte[] { 245, 106, 51, 62, 230, 102, 12, 236, 247, 133, 232, 133, 128, 238, 143, 181, 216, 141, 253, 28, 152, 212, 58, 244, 126, 91, 106, 67, 221, 99, 20, 213, 22, 66, 196, 240, 55, 169, 87, 153, 0, 223, 176, 169, 65, 28, 243, 110, 138, 24, 22, 25, 110, 206, 77, 41, 97, 139, 68, 137, 124, 237, 229, 0 },
                            PasswordSalt = new byte[] { 118, 189, 163, 244, 10, 253, 78, 59, 73, 222, 168, 103, 176, 233, 50, 248, 239, 255, 163, 203, 223, 121, 192, 64, 222, 144, 75, 171, 167, 134, 239, 195, 192, 192, 149, 46, 116, 118, 194, 128, 15, 4, 247, 77, 11, 121, 58, 127, 108, 85, 239, 211, 64, 122, 227, 136, 91, 116, 69, 61, 17, 246, 247, 150, 130, 189, 87, 220, 132, 205, 147, 27, 24, 144, 17, 217, 16, 129, 154, 24, 142, 181, 166, 110, 74, 208, 50, 71, 211, 238, 179, 101, 86, 69, 182, 161, 121, 80, 165, 113, 238, 31, 45, 142, 26, 225, 62, 82, 106, 238, 182, 222, 164, 162, 85, 80, 91, 67, 244, 140, 102, 45, 148, 184, 141, 112, 59, 144 },
                            Role = "Admin"
                        },
                        new
                        {
                            UserId = 2,
                            Email = "user",
                            PasswordHash = new byte[] { 205, 242, 200, 226, 213, 228, 240, 188, 243, 151, 121, 35, 228, 176, 29, 196, 44, 198, 35, 213, 82, 239, 75, 64, 209, 152, 166, 216, 255, 37, 186, 217, 5, 35, 62, 153, 213, 240, 102, 34, 85, 180, 231, 238, 48, 69, 45, 0, 141, 62, 193, 127, 220, 130, 93, 146, 223, 79, 50, 198, 231, 213, 12, 96 },
                            PasswordSalt = new byte[] { 96, 184, 191, 169, 133, 56, 226, 228, 79, 162, 202, 100, 125, 93, 185, 169, 3, 204, 222, 143, 157, 180, 221, 163, 178, 185, 114, 215, 58, 58, 76, 181, 246, 151, 191, 23, 245, 159, 221, 224, 150, 15, 99, 81, 175, 60, 82, 2, 154, 238, 182, 207, 203, 251, 58, 132, 152, 27, 35, 29, 90, 122, 229, 144, 94, 56, 201, 50, 124, 118, 176, 240, 147, 100, 135, 81, 133, 84, 6, 230, 86, 70, 113, 181, 213, 84, 228, 175, 162, 18, 136, 102, 186, 51, 193, 215, 16, 106, 3, 68, 73, 165, 8, 50, 229, 151, 74, 73, 90, 183, 126, 211, 2, 90, 99, 72, 136, 187, 69, 80, 87, 181, 236, 177, 253, 251, 2, 90 },
                            Role = "User"
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

            modelBuilder.Entity("Backend.Models.SalaryModel", b =>
                {
                    b.HasOne("Backend.Models.EmployeeModel", null)
                        .WithMany("Salaries")
                        .HasForeignKey("EmployeeID");
                });

            modelBuilder.Entity("Backend.Models.User", b =>
                {
                    b.HasOne("Backend.Models.CompanyModel", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyID");

                    b.Navigation("Company");
                });

            modelBuilder.Entity("Backend.Models.EmployeeModel", b =>
                {
                    b.Navigation("Salaries");
                });
#pragma warning restore 612, 618
        }
    }
}
