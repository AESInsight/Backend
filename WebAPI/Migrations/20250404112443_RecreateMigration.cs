using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class RecreateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "JobTitle",
                table: "Employee",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Employee",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "Employee",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeID",
                table: "Employee",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "Company",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<byte[]>(type: "longblob", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "longblob", nullable: false),
                    Role = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "PasswordHash", "PasswordSalt", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new byte[] { 89, 142, 137, 64, 69, 220, 79, 95, 65, 119, 230, 147, 223, 237, 33, 152, 238, 212, 1, 67, 236, 210, 115, 187, 19, 157, 42, 206, 147, 100, 79, 176, 207, 3, 76, 11, 197, 81, 53, 170, 190, 208, 236, 25, 194, 43, 158, 22, 8, 170, 201, 36, 161, 129, 86, 139, 145, 162, 203, 163, 11, 15, 15, 176 }, new byte[] { 203, 191, 238, 60, 127, 183, 3, 230, 216, 57, 104, 186, 92, 42, 108, 19, 61, 100, 135, 181, 219, 54, 191, 231, 223, 50, 134, 142, 182, 169, 149, 17, 106, 147, 241, 156, 208, 178, 44, 207, 28, 151, 174, 62, 70, 175, 39, 106, 140, 209, 121, 115, 239, 106, 19, 58, 93, 147, 23, 102, 176, 126, 219, 25, 106, 131, 225, 73, 86, 55, 23, 166, 22, 157, 68, 106, 136, 28, 183, 51, 19, 54, 38, 159, 206, 55, 59, 242, 136, 74, 34, 111, 116, 250, 178, 231, 44, 168, 36, 45, 235, 99, 228, 247, 90, 74, 85, 134, 6, 182, 79, 217, 188, 179, 177, 232, 233, 138, 31, 76, 135, 208, 53, 43, 71, 201, 254, 64 }, "Admin", "admin" },
                    { 2, new byte[] { 11, 124, 221, 185, 101, 181, 127, 85, 162, 20, 205, 238, 211, 159, 48, 240, 69, 88, 245, 195, 76, 3, 75, 88, 164, 165, 231, 186, 226, 205, 250, 88, 166, 166, 145, 71, 152, 186, 154, 137, 114, 6, 232, 101, 2, 127, 28, 86, 112, 41, 35, 195, 222, 152, 67, 93, 130, 48, 172, 152, 219, 69, 186, 41 }, new byte[] { 203, 191, 238, 60, 127, 183, 3, 230, 216, 57, 104, 186, 92, 42, 108, 19, 61, 100, 135, 181, 219, 54, 191, 231, 223, 50, 134, 142, 182, 169, 149, 17, 106, 147, 241, 156, 208, 178, 44, 207, 28, 151, 174, 62, 70, 175, 39, 106, 140, 209, 121, 115, 239, 106, 19, 58, 93, 147, 23, 102, 176, 126, 219, 25, 106, 131, 225, 73, 86, 55, 23, 166, 22, 157, 68, 106, 136, 28, 183, 51, 19, 54, 38, 159, 206, 55, 59, 242, 136, 74, 34, 111, 116, 250, 178, 231, 44, 168, 36, 45, 235, 99, 228, 247, 90, 74, 85, 134, 6, 182, 79, 217, 188, 179, 177, 232, 233, 138, 31, 76, 135, 208, 53, 43, 71, 201, 254, 64 }, "User", "user" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CompanyID",
                table: "Employee",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Company_CompanyID",
                table: "Employee",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "CompanyID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Company_CompanyID",
                table: "Employee");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Employee_CompanyID",
                table: "Employee");

            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "JobTitle",
                keyValue: null,
                column: "JobTitle",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "JobTitle",
                table: "Employee",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Gender",
                keyValue: null,
                column: "Gender",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Employee",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyID",
                table: "Employee",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeID",
                table: "Employee",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "Company",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
