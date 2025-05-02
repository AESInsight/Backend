using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class CompanyPasswordRemove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "Company",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CVR",
                table: "Company",
                type: "varchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CompanyID", "PasswordHash", "PasswordSalt" },
                values: new object[] { null, new byte[] { 76, 177, 182, 188, 24, 127, 115, 188, 210, 201, 63, 62, 196, 108, 113, 227, 242, 210, 203, 176, 163, 76, 107, 126, 216, 233, 170, 148, 218, 251, 102, 168, 93, 51, 176, 92, 74, 79, 225, 219, 153, 121, 86, 242, 125, 214, 147, 245, 168, 70, 38, 121, 167, 147, 146, 245, 244, 142, 50, 235, 64, 106, 1, 232 }, new byte[] { 255, 212, 21, 224, 152, 62, 47, 102, 199, 72, 105, 255, 30, 239, 254, 187, 154, 232, 136, 212, 91, 54, 252, 92, 34, 177, 149, 8, 214, 25, 36, 184, 147, 249, 53, 104, 3, 152, 255, 81, 140, 219, 143, 254, 169, 68, 152, 77, 250, 105, 120, 234, 198, 235, 246, 90, 180, 102, 207, 222, 134, 103, 135, 101, 243, 62, 186, 202, 219, 168, 137, 171, 183, 237, 18, 9, 38, 192, 184, 106, 50, 197, 201, 232, 234, 127, 211, 234, 25, 20, 146, 150, 7, 46, 108, 110, 182, 202, 65, 161, 7, 69, 177, 143, 88, 129, 12, 232, 9, 254, 63, 242, 173, 78, 22, 40, 26, 101, 25, 22, 211, 10, 171, 178, 183, 32, 77, 153 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "CompanyID", "PasswordHash", "PasswordSalt" },
                values: new object[] { null, new byte[] { 136, 17, 47, 144, 162, 229, 203, 26, 20, 43, 53, 68, 226, 154, 66, 177, 67, 136, 169, 153, 144, 254, 80, 247, 242, 194, 208, 173, 166, 211, 155, 232, 133, 162, 15, 209, 162, 246, 95, 157, 58, 165, 122, 217, 69, 202, 251, 43, 69, 87, 133, 184, 59, 18, 164, 145, 59, 63, 85, 248, 173, 171, 131, 122 }, new byte[] { 238, 108, 154, 201, 142, 184, 206, 136, 234, 93, 130, 126, 192, 154, 47, 225, 68, 144, 194, 180, 170, 192, 95, 120, 224, 39, 202, 186, 104, 103, 68, 190, 113, 13, 185, 97, 254, 64, 228, 237, 204, 7, 151, 222, 182, 200, 87, 99, 171, 232, 28, 59, 167, 20, 69, 244, 245, 196, 82, 35, 2, 101, 88, 33, 51, 100, 10, 16, 93, 51, 24, 50, 31, 160, 165, 170, 159, 164, 165, 20, 4, 119, 212, 164, 163, 104, 205, 154, 124, 244, 42, 115, 112, 107, 179, 168, 20, 232, 87, 60, 66, 49, 95, 27, 186, 170, 156, 38, 176, 30, 160, 254, 162, 11, 137, 168, 176, 54, 75, 143, 52, 108, 114, 231, 134, 200, 0, 210 } });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyID",
                table: "Users",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Company_CompanyID",
                table: "Users",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "CompanyID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Company_CompanyID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CompanyID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "Company",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CVR",
                table: "Company",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldMaxLength: 8)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

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
        }
    }
}
