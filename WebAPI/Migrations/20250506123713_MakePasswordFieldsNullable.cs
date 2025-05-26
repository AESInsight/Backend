using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class MakePasswordFieldsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordSalt",
                table: "Company",
                type: "longblob",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "longblob");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordHash",
                table: "Company",
                type: "longblob",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "longblob");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 137, 74, 61, 132, 4, 158, 111, 65, 172, 62, 198, 159, 48, 178, 121, 222, 185, 103, 9, 127, 102, 3, 62, 54, 21, 12, 43, 187, 133, 8, 188, 35, 251, 129, 136, 135, 50, 79, 17, 115, 187, 62, 224, 243, 237, 69, 184, 11, 125, 171, 252, 255, 57, 180, 138, 44, 208, 48, 217, 26, 33, 122, 33, 46 }, new byte[] { 147, 5, 163, 31, 205, 71, 173, 186, 200, 86, 50, 154, 219, 45, 160, 188, 8, 146, 24, 243, 41, 48, 190, 41, 169, 80, 18, 27, 156, 228, 145, 225, 122, 188, 128, 166, 191, 32, 142, 196, 176, 142, 139, 95, 232, 189, 10, 202, 173, 215, 250, 243, 68, 62, 212, 66, 75, 180, 199, 224, 18, 86, 241, 112, 244, 24, 173, 65, 222, 186, 194, 226, 166, 204, 139, 48, 165, 86, 179, 112, 64, 102, 14, 135, 227, 185, 249, 193, 99, 79, 182, 62, 124, 214, 11, 116, 118, 44, 190, 223, 249, 73, 36, 209, 27, 186, 117, 146, 224, 183, 54, 85, 151, 201, 58, 112, 65, 27, 55, 243, 27, 234, 36, 30, 246, 152, 210, 132 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 199, 5, 58, 1, 107, 224, 17, 72, 228, 77, 77, 175, 90, 84, 21, 128, 160, 200, 1, 10, 2, 113, 143, 4, 189, 145, 196, 103, 216, 102, 164, 203, 92, 56, 235, 198, 187, 97, 209, 180, 49, 32, 125, 139, 110, 160, 134, 24, 244, 184, 41, 210, 16, 49, 74, 216, 254, 58, 124, 38, 77, 194, 42, 66 }, new byte[] { 192, 105, 204, 89, 28, 143, 203, 96, 85, 143, 109, 111, 96, 35, 111, 44, 46, 253, 3, 122, 69, 141, 160, 0, 97, 194, 97, 19, 26, 62, 185, 6, 13, 9, 184, 71, 193, 192, 196, 47, 125, 201, 73, 79, 52, 169, 11, 58, 9, 66, 126, 99, 176, 131, 54, 214, 151, 15, 85, 48, 90, 242, 106, 73, 98, 223, 7, 231, 93, 184, 42, 33, 12, 212, 34, 57, 225, 18, 124, 52, 172, 108, 131, 142, 178, 140, 222, 199, 207, 44, 173, 125, 86, 198, 221, 211, 114, 172, 222, 108, 212, 117, 186, 204, 251, 71, 210, 51, 112, 223, 28, 73, 222, 133, 213, 86, 65, 222, 188, 73, 91, 121, 72, 28, 121, 187, 127, 160 } });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordSalt",
                table: "Company",
                type: "longblob",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "longblob",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordHash",
                table: "Company",
                type: "longblob",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "longblob",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 206, 51, 30, 203, 111, 28, 201, 119, 129, 31, 99, 151, 222, 58, 238, 213, 143, 22, 49, 167, 102, 134, 33, 7, 46, 81, 146, 55, 81, 152, 208, 252, 197, 225, 226, 255, 151, 18, 11, 50, 25, 54, 148, 81, 113, 106, 85, 215, 204, 113, 122, 97, 167, 35, 240, 79, 2, 227, 31, 235, 162, 94, 131, 244 }, new byte[] { 194, 140, 188, 192, 167, 203, 209, 218, 247, 171, 162, 11, 254, 204, 221, 141, 180, 16, 186, 198, 178, 166, 48, 27, 64, 220, 28, 206, 174, 163, 116, 238, 222, 210, 219, 47, 78, 172, 81, 21, 40, 1, 240, 77, 55, 57, 133, 128, 189, 95, 36, 182, 228, 167, 248, 80, 166, 30, 160, 243, 6, 65, 203, 107, 189, 55, 51, 215, 113, 230, 110, 147, 150, 248, 192, 170, 4, 138, 95, 147, 215, 158, 237, 219, 58, 36, 72, 142, 35, 38, 228, 161, 222, 150, 234, 34, 16, 149, 1, 154, 62, 57, 100, 97, 198, 216, 122, 211, 247, 118, 148, 98, 248, 68, 145, 83, 43, 253, 118, 158, 120, 90, 102, 129, 119, 150, 42, 77 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 184, 138, 27, 80, 125, 145, 137, 156, 149, 157, 149, 6, 43, 10, 224, 35, 179, 91, 129, 211, 116, 158, 243, 203, 92, 48, 82, 185, 146, 103, 87, 253, 250, 227, 201, 11, 104, 192, 164, 99, 77, 154, 197, 124, 81, 19, 28, 30, 26, 204, 204, 99, 10, 136, 177, 28, 95, 220, 236, 53, 96, 147, 221, 15 }, new byte[] { 15, 49, 226, 173, 239, 33, 19, 117, 225, 197, 226, 208, 232, 141, 23, 135, 188, 195, 225, 224, 226, 182, 138, 217, 133, 169, 180, 214, 194, 24, 178, 228, 29, 35, 104, 5, 220, 177, 249, 139, 28, 140, 232, 227, 71, 127, 50, 51, 87, 210, 89, 199, 66, 160, 207, 208, 188, 253, 70, 42, 27, 58, 167, 143, 112, 23, 76, 236, 18, 195, 52, 240, 234, 160, 142, 71, 65, 90, 229, 94, 45, 247, 91, 113, 118, 68, 213, 101, 82, 240, 43, 180, 173, 189, 230, 241, 32, 5, 180, 105, 128, 252, 94, 41, 224, 1, 117, 38, 218, 122, 251, 217, 236, 47, 55, 97, 168, 24, 91, 9, 243, 240, 214, 104, 200, 220, 221, 186 } });
        }
    }
}
