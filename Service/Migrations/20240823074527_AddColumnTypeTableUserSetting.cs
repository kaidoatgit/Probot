using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProPayments.Service.Migrations
{
    public partial class AddColumnTypeTableUserSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "UserSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 23, 7, 45, 26, 700, DateTimeKind.Utc).AddTicks(6728));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 23, 7, 45, 26, 700, DateTimeKind.Utc).AddTicks(6737));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 23, 7, 45, 26, 700, DateTimeKind.Utc).AddTicks(6738));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserSettings");

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 22, 5, 36, 33, 307, DateTimeKind.Utc).AddTicks(3827));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 22, 5, 36, 33, 307, DateTimeKind.Utc).AddTicks(3847));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 22, 5, 36, 33, 307, DateTimeKind.Utc).AddTicks(3849));
        }
    }
}
