using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class CreateSalaryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Employee");

            migrationBuilder.CreateTable(
                name: "Salaries",
                columns: table => new
                {
                    SalaryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<double>(type: "double", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaries", x => x.SalaryID);
                    table.ForeignKey(
                        name: "FK_Salaries_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 83, 202, 3, 155, 49, 125, 187, 177, 218, 10, 105, 82, 160, 68, 236, 103, 157, 212, 14, 178, 130, 119, 164, 55, 142, 207, 125, 86, 145, 73, 221, 15, 219, 249, 12, 54, 124, 214, 131, 140, 35, 221, 162, 249, 62, 93, 128, 242, 91, 87, 11, 149, 118, 148, 190, 101, 238, 135, 99, 98, 75, 233, 34, 18 }, new byte[] { 0, 191, 171, 40, 56, 115, 125, 3, 156, 132, 195, 198, 154, 134, 111, 213, 128, 212, 49, 226, 100, 212, 173, 194, 171, 117, 135, 109, 124, 68, 159, 255, 49, 52, 241, 85, 146, 28, 221, 118, 154, 145, 125, 189, 151, 72, 43, 88, 170, 131, 187, 81, 205, 180, 30, 20, 32, 121, 158, 84, 42, 89, 129, 188, 59, 213, 176, 206, 227, 18, 103, 191, 146, 165, 141, 250, 253, 193, 87, 157, 107, 195, 244, 130, 204, 6, 3, 178, 18, 41, 28, 62, 190, 214, 220, 23, 25, 53, 157, 127, 83, 174, 125, 51, 106, 28, 67, 10, 61, 88, 251, 8, 250, 143, 77, 18, 153, 214, 111, 225, 229, 237, 105, 118, 26, 173, 30, 213 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 11, 54, 233, 25, 88, 143, 40, 125, 104, 31, 189, 6, 216, 217, 127, 20, 31, 222, 255, 25, 52, 142, 72, 165, 100, 89, 235, 96, 255, 148, 176, 58, 139, 251, 250, 226, 144, 69, 182, 136, 201, 210, 190, 39, 213, 97, 120, 170, 110, 9, 207, 208, 142, 234, 230, 252, 163, 107, 183, 4, 108, 117, 10, 152 }, new byte[] { 158, 236, 164, 79, 129, 121, 17, 188, 212, 40, 225, 158, 115, 38, 6, 231, 188, 249, 161, 114, 226, 168, 33, 16, 162, 92, 204, 138, 167, 255, 26, 98, 94, 2, 20, 243, 219, 34, 46, 214, 170, 46, 121, 110, 31, 228, 147, 50, 161, 23, 231, 41, 131, 137, 133, 90, 178, 60, 167, 248, 23, 184, 231, 242, 250, 131, 97, 238, 206, 87, 78, 6, 38, 203, 159, 78, 12, 200, 224, 216, 172, 74, 73, 83, 57, 152, 112, 87, 160, 153, 27, 183, 159, 208, 206, 13, 201, 23, 12, 121, 206, 21, 115, 200, 75, 233, 209, 172, 69, 175, 215, 81, 89, 83, 23, 174, 90, 142, 189, 0, 227, 141, 93, 181, 31, 246, 91, 210 } });

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_EmployeeID",
                table: "Salaries",
                column: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Salaries");

            migrationBuilder.AddColumn<double>(
                name: "Salary",
                table: "Employee",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 164, 12, 98, 104, 208, 80, 141, 31, 62, 133, 170, 157, 198, 6, 33, 138, 135, 129, 232, 126, 55, 243, 16, 198, 213, 58, 230, 185, 146, 167, 53, 217, 137, 18, 201, 111, 219, 227, 170, 245, 229, 6, 65, 133, 53, 94, 107, 130, 202, 125, 116, 165, 248, 253, 32, 238, 155, 207, 244, 87, 103, 44, 27, 205 }, new byte[] { 184, 176, 182, 182, 35, 203, 148, 61, 200, 173, 206, 176, 162, 215, 193, 99, 56, 217, 202, 102, 3, 248, 124, 195, 229, 11, 237, 77, 96, 107, 65, 91, 201, 215, 227, 229, 170, 17, 205, 178, 111, 248, 124, 17, 9, 126, 104, 205, 200, 105, 168, 80, 156, 28, 238, 35, 29, 139, 0, 237, 48, 241, 214, 148, 122, 170, 99, 245, 91, 176, 175, 199, 170, 62, 17, 169, 237, 83, 11, 85, 27, 212, 136, 32, 197, 170, 44, 25, 252, 195, 76, 33, 225, 215, 35, 103, 187, 6, 141, 171, 190, 227, 49, 248, 139, 43, 227, 245, 44, 24, 26, 188, 9, 135, 193, 65, 145, 4, 58, 253, 132, 220, 45, 31, 235, 158, 43, 120 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 240, 29, 184, 243, 121, 65, 109, 162, 216, 103, 179, 90, 126, 169, 54, 108, 22, 37, 111, 147, 59, 210, 29, 141, 12, 226, 233, 192, 193, 154, 108, 240, 116, 115, 46, 32, 85, 195, 203, 22, 16, 132, 97, 41, 19, 176, 142, 79, 202, 248, 1, 80, 162, 93, 169, 137, 168, 239, 233, 228, 134, 162, 44, 168 }, new byte[] { 29, 13, 128, 104, 189, 125, 135, 223, 119, 163, 237, 148, 67, 184, 79, 177, 161, 149, 226, 130, 75, 54, 164, 224, 181, 162, 211, 191, 177, 101, 73, 226, 18, 172, 70, 199, 56, 212, 14, 20, 127, 159, 112, 136, 81, 203, 160, 153, 113, 87, 185, 63, 92, 237, 69, 252, 229, 137, 253, 66, 6, 139, 144, 81, 128, 86, 170, 21, 153, 66, 206, 152, 31, 225, 156, 187, 194, 175, 87, 73, 98, 99, 254, 24, 184, 131, 239, 25, 123, 108, 159, 209, 155, 29, 91, 5, 193, 73, 1, 132, 126, 207, 11, 96, 134, 48, 14, 181, 42, 83, 202, 225, 41, 248, 194, 56, 234, 112, 37, 103, 179, 150, 177, 132, 107, 120, 234, 80 } });
        }
    }
}
