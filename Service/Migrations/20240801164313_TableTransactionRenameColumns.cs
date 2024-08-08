using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProPayments.Service.Migrations
{
    public partial class TableTransactionRenameColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionHash",
                table: "Transactions",
                newName: "Hash");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Transactions",
                newName: "Id");

            migrationBuilder.UpdateData(
                table: "PlanOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 1, 16, 43, 13, 195, DateTimeKind.Utc).AddTicks(603));

            migrationBuilder.UpdateData(
                table: "PlanOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 1, 16, 43, 13, 195, DateTimeKind.Utc).AddTicks(606));

            migrationBuilder.UpdateData(
                table: "PlanOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 1, 16, 43, 13, 195, DateTimeKind.Utc).AddTicks(610));

            migrationBuilder.UpdateData(
                table: "PlanOptions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 1, 16, 43, 13, 195, DateTimeKind.Utc).AddTicks(611));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Hash",
                table: "Transactions",
                newName: "TransactionHash");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Transactions",
                newName: "TransactionId");

            migrationBuilder.UpdateData(
                table: "PlanOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 1, 15, 32, 35, 307, DateTimeKind.Utc).AddTicks(2507));

            migrationBuilder.UpdateData(
                table: "PlanOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 1, 15, 32, 35, 307, DateTimeKind.Utc).AddTicks(2511));

            migrationBuilder.UpdateData(
                table: "PlanOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 1, 15, 32, 35, 307, DateTimeKind.Utc).AddTicks(2514));

            migrationBuilder.UpdateData(
                table: "PlanOptions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 1, 15, 32, 35, 307, DateTimeKind.Utc).AddTicks(2516));
        }
    }
}
