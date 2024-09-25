using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Probot.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamingColumnsTokenToCoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Transactions",
                newName: "Coin");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Invoices",
                newName: "Coin");

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 19, 1, 16, 7, 540, DateTimeKind.Utc).AddTicks(7808));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 19, 1, 16, 7, 540, DateTimeKind.Utc).AddTicks(7815));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 19, 1, 16, 7, 540, DateTimeKind.Utc).AddTicks(7816));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Coin",
                table: "Transactions",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "Coin",
                table: "Invoices",
                newName: "Token");

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 19, 1, 9, 46, 242, DateTimeKind.Utc).AddTicks(9111));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 19, 1, 9, 46, 242, DateTimeKind.Utc).AddTicks(9119));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 19, 1, 9, 46, 242, DateTimeKind.Utc).AddTicks(9121));
        }
    }
}
