using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class databaseV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeModelEmployeeID",
                table: "Salaries",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 38, 199, 242, 244, 192, 43, 175, 111, 47, 127, 228, 88, 234, 54, 197, 214, 201, 8, 158, 28, 215, 233, 136, 200, 112, 26, 101, 48, 245, 130, 158, 68, 153, 56, 158, 118, 166, 87, 205, 80, 211, 255, 28, 31, 227, 13, 9, 30, 240, 222, 28, 150, 91, 177, 23, 104, 175, 94, 251, 131, 16, 114, 39, 234 }, new byte[] { 167, 122, 67, 22, 17, 244, 185, 192, 119, 188, 208, 12, 116, 120, 68, 62, 175, 113, 132, 73, 29, 125, 27, 121, 102, 20, 175, 71, 228, 133, 55, 82, 173, 188, 3, 34, 208, 145, 21, 157, 187, 1, 204, 49, 174, 59, 122, 185, 96, 24, 51, 101, 177, 167, 142, 69, 102, 161, 37, 68, 213, 95, 7, 194, 192, 181, 180, 18, 87, 104, 130, 64, 185, 79, 207, 187, 94, 126, 61, 133, 27, 61, 208, 45, 3, 144, 210, 7, 73, 51, 210, 238, 174, 57, 151, 10, 225, 133, 129, 195, 28, 124, 230, 82, 150, 13, 178, 166, 23, 180, 68, 116, 159, 102, 204, 24, 243, 236, 255, 66, 235, 182, 56, 173, 47, 48, 179, 6 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 169, 178, 239, 70, 74, 207, 195, 209, 193, 121, 215, 188, 22, 176, 198, 144, 248, 58, 167, 20, 159, 13, 160, 152, 9, 138, 159, 91, 3, 73, 226, 236, 86, 175, 107, 236, 50, 101, 252, 131, 208, 50, 126, 254, 30, 52, 158, 66, 74, 58, 145, 152, 228, 115, 67, 163, 210, 133, 75, 100, 175, 204, 221, 155 }, new byte[] { 48, 98, 240, 87, 252, 45, 202, 82, 140, 30, 249, 48, 199, 103, 10, 225, 199, 176, 46, 35, 225, 89, 131, 100, 1, 186, 50, 108, 153, 54, 105, 227, 61, 98, 66, 182, 69, 143, 129, 98, 87, 82, 244, 99, 64, 248, 174, 179, 199, 240, 154, 132, 176, 34, 157, 8, 201, 221, 73, 160, 108, 68, 4, 183, 123, 9, 70, 10, 245, 180, 70, 178, 247, 248, 12, 45, 15, 133, 87, 198, 180, 83, 156, 108, 169, 121, 133, 157, 196, 35, 59, 121, 174, 116, 182, 180, 101, 228, 222, 192, 251, 70, 42, 124, 246, 33, 15, 83, 5, 59, 106, 204, 200, 215, 80, 229, 255, 44, 172, 248, 59, 235, 139, 176, 208, 213, 59, 10 } });

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_EmployeeModelEmployeeID",
                table: "Salaries",
                column: "EmployeeModelEmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Salaries_Employee_EmployeeModelEmployeeID",
                table: "Salaries",
                column: "EmployeeModelEmployeeID",
                principalTable: "Employee",
                principalColumn: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salaries_Employee_EmployeeModelEmployeeID",
                table: "Salaries");

            migrationBuilder.DropIndex(
                name: "IX_Salaries_EmployeeModelEmployeeID",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "EmployeeModelEmployeeID",
                table: "Salaries");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 39, 99, 148, 185, 57, 129, 240, 206, 39, 18, 122, 154, 148, 1, 22, 7, 106, 253, 254, 85, 140, 61, 116, 27, 30, 128, 49, 112, 116, 244, 215, 159, 33, 212, 196, 24, 87, 43, 77, 65, 40, 160, 187, 12, 27, 62, 108, 155, 203, 236, 137, 93, 120, 149, 110, 16, 194, 7, 169, 188, 26, 42, 199, 245 }, new byte[] { 91, 91, 79, 229, 160, 70, 179, 57, 223, 227, 196, 245, 173, 246, 160, 99, 176, 78, 213, 11, 26, 181, 251, 198, 71, 171, 124, 155, 38, 10, 62, 8, 14, 97, 19, 74, 34, 67, 157, 89, 91, 83, 135, 74, 65, 33, 75, 31, 53, 23, 223, 180, 83, 193, 25, 125, 22, 173, 238, 39, 10, 144, 123, 202, 67, 248, 47, 164, 14, 227, 147, 136, 105, 69, 152, 217, 168, 94, 47, 252, 191, 61, 94, 141, 38, 16, 57, 9, 148, 108, 105, 175, 134, 150, 252, 192, 173, 89, 100, 93, 77, 170, 108, 28, 182, 215, 58, 0, 3, 145, 28, 1, 158, 164, 221, 200, 66, 70, 125, 175, 240, 88, 185, 43, 211, 169, 65, 159 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 30, 179, 124, 61, 233, 105, 23, 115, 13, 189, 21, 74, 212, 149, 85, 158, 21, 29, 218, 77, 27, 229, 52, 137, 250, 174, 221, 119, 245, 46, 83, 90, 1, 225, 112, 170, 8, 99, 92, 210, 244, 42, 120, 178, 21, 1, 103, 66, 184, 17, 143, 36, 167, 104, 163, 154, 87, 146, 226, 237, 88, 175, 128, 245 }, new byte[] { 81, 56, 168, 247, 4, 252, 91, 185, 31, 189, 94, 198, 166, 234, 162, 210, 84, 67, 105, 145, 189, 146, 166, 161, 156, 206, 80, 67, 56, 164, 109, 9, 31, 188, 200, 106, 16, 252, 224, 226, 161, 174, 96, 50, 254, 134, 250, 22, 235, 37, 45, 78, 134, 98, 97, 183, 250, 235, 254, 16, 51, 134, 20, 92, 9, 230, 205, 180, 253, 99, 137, 119, 126, 68, 89, 248, 220, 36, 40, 93, 207, 213, 24, 234, 186, 187, 255, 115, 70, 149, 129, 195, 23, 132, 6, 120, 11, 204, 112, 171, 100, 206, 90, 44, 110, 10, 90, 253, 73, 196, 201, 141, 36, 252, 153, 47, 35, 191, 163, 55, 33, 32, 57, 192, 157, 62, 105, 97 } });
        }
    }
}
