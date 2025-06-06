﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class AddResetPasswordFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordToken",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordTokenExpiry",
                table: "Users",
                type: "datetime(6)",
                nullable: true);



            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CompanyID", "PasswordHash", "PasswordSalt", "ResetPasswordToken", "ResetPasswordTokenExpiry" },
                values: new object[] { null, new byte[] { 176, 167, 142, 33, 245, 16, 111, 25, 162, 177, 106, 93, 113, 172, 188, 168, 100, 154, 231, 34, 21, 165, 193, 43, 106, 112, 180, 20, 202, 208, 165, 248, 153, 25, 245, 189, 11, 205, 25, 0, 209, 149, 163, 42, 14, 247, 135, 140, 102, 15, 231, 28, 147, 186, 38, 61, 106, 196, 162, 197, 64, 133, 43, 179 }, new byte[] { 67, 5, 92, 80, 12, 108, 38, 19, 145, 37, 186, 64, 52, 33, 21, 38, 73, 101, 187, 91, 201, 53, 1, 96, 171, 216, 163, 37, 72, 110, 193, 14, 190, 40, 4, 176, 237, 198, 66, 111, 160, 209, 237, 143, 243, 2, 33, 166, 176, 112, 242, 240, 86, 0, 65, 137, 33, 169, 143, 95, 179, 96, 155, 103, 153, 112, 31, 188, 141, 127, 189, 151, 70, 154, 50, 210, 211, 122, 202, 243, 31, 21, 77, 94, 93, 166, 42, 165, 73, 102, 52, 44, 93, 17, 247, 63, 196, 195, 183, 16, 180, 86, 113, 164, 132, 195, 27, 26, 182, 188, 191, 161, 203, 67, 183, 97, 65, 109, 21, 123, 66, 149, 211, 149, 240, 97, 177, 170 }, null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "CompanyID", "PasswordHash", "PasswordSalt", "ResetPasswordToken", "ResetPasswordTokenExpiry" },
                values: new object[] { null, new byte[] { 196, 4, 66, 117, 175, 69, 137, 223, 198, 156, 14, 187, 214, 13, 8, 203, 5, 15, 120, 82, 33, 155, 190, 90, 4, 40, 115, 112, 49, 198, 138, 213, 133, 246, 187, 113, 163, 196, 248, 178, 158, 209, 178, 57, 72, 55, 121, 53, 62, 95, 121, 250, 89, 133, 149, 155, 148, 215, 118, 65, 108, 252, 254, 252 }, new byte[] { 125, 115, 186, 39, 24, 220, 46, 233, 186, 234, 3, 62, 163, 84, 27, 41, 203, 235, 148, 112, 236, 160, 113, 113, 132, 120, 97, 255, 53, 56, 152, 228, 61, 25, 121, 165, 3, 98, 30, 86, 9, 56, 167, 35, 42, 220, 200, 170, 52, 58, 205, 234, 96, 116, 66, 44, 221, 170, 162, 88, 176, 11, 183, 224, 47, 208, 188, 79, 79, 83, 72, 107, 48, 172, 29, 215, 149, 146, 233, 191, 143, 155, 16, 203, 98, 57, 23, 74, 209, 121, 129, 32, 150, 133, 162, 33, 220, 21, 165, 184, 162, 228, 123, 224, 255, 201, 234, 80, 102, 120, 25, 178, 213, 101, 156, 42, 96, 196, 237, 62, 163, 83, 107, 97, 170, 90, 214, 65 }, null, null });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "ResetPasswordToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetPasswordTokenExpiry",
                table: "Users");



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
