﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailPasswordField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailPassword",
                table: "Company");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailPassword",
                table: "Company",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 89, 142, 137, 64, 69, 220, 79, 95, 65, 119, 230, 147, 223, 237, 33, 152, 238, 212, 1, 67, 236, 210, 115, 187, 19, 157, 42, 206, 147, 100, 79, 176, 207, 3, 76, 11, 197, 81, 53, 170, 190, 208, 236, 25, 194, 43, 158, 22, 8, 170, 201, 36, 161, 129, 86, 139, 145, 162, 203, 163, 11, 15, 15, 176 }, new byte[] { 203, 191, 238, 60, 127, 183, 3, 230, 216, 57, 104, 186, 92, 42, 108, 19, 61, 100, 135, 181, 219, 54, 191, 231, 223, 50, 134, 142, 182, 169, 149, 17, 106, 147, 241, 156, 208, 178, 44, 207, 28, 151, 174, 62, 70, 175, 39, 106, 140, 209, 121, 115, 239, 106, 19, 58, 93, 147, 23, 102, 176, 126, 219, 25, 106, 131, 225, 73, 86, 55, 23, 166, 22, 157, 68, 106, 136, 28, 183, 51, 19, 54, 38, 159, 206, 55, 59, 242, 136, 74, 34, 111, 116, 250, 178, 231, 44, 168, 36, 45, 235, 99, 228, 247, 90, 74, 85, 134, 6, 182, 79, 217, 188, 179, 177, 232, 233, 138, 31, 76, 135, 208, 53, 43, 71, 201, 254, 64 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 11, 124, 221, 185, 101, 181, 127, 85, 162, 20, 205, 238, 211, 159, 48, 240, 69, 88, 245, 195, 76, 3, 75, 88, 164, 165, 231, 186, 226, 205, 250, 88, 166, 166, 145, 71, 152, 186, 154, 137, 114, 6, 232, 101, 2, 127, 28, 86, 112, 41, 35, 195, 222, 152, 67, 93, 130, 48, 172, 152, 219, 69, 186, 41 }, new byte[] { 203, 191, 238, 60, 127, 183, 3, 230, 216, 57, 104, 186, 92, 42, 108, 19, 61, 100, 135, 181, 219, 54, 191, 231, 223, 50, 134, 142, 182, 169, 149, 17, 106, 147, 241, 156, 208, 178, 44, 207, 28, 151, 174, 62, 70, 175, 39, 106, 140, 209, 121, 115, 239, 106, 19, 58, 93, 147, 23, 102, 176, 126, 219, 25, 106, 131, 225, 73, 86, 55, 23, 166, 22, 157, 68, 106, 136, 28, 183, 51, 19, 54, 38, 159, 206, 55, 59, 242, 136, 74, 34, 111, 116, 250, 178, 231, 44, 168, 36, 45, 235, 99, 228, 247, 90, 74, 85, 134, 6, 182, 79, 217, 188, 179, 177, 232, 233, 138, 31, 76, 135, 208, 53, 43, 71, 201, 254, 64 } });
        }
    }
}
