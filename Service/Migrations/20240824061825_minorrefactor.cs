using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProPayments.Service.Migrations
{
    public partial class minorrefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 24, 6, 18, 24, 728, DateTimeKind.Utc).AddTicks(1912));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 24, 6, 18, 24, 728, DateTimeKind.Utc).AddTicks(1921));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 24, 6, 18, 24, 728, DateTimeKind.Utc).AddTicks(1923));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 23, 10, 43, 46, 585, DateTimeKind.Utc).AddTicks(4700));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 23, 10, 43, 46, 585, DateTimeKind.Utc).AddTicks(4712));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 23, 10, 43, 46, 585, DateTimeKind.Utc).AddTicks(4714));
        }
    }
}
