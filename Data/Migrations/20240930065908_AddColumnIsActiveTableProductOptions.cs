using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Probot.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnIsActiveTableProductOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductKeys_OrderItems_OrderItemId",
                table: "ProductKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductKeys_Users_UserId",
                table: "ProductKeys");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProductOptions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "ProductKeys",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<ulong>(
                name: "OrderItemId",
                table: "ProductKeys",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IsActive" },
                values: new object[] { new DateTime(2024, 9, 30, 6, 59, 8, 496, DateTimeKind.Utc).AddTicks(3583), true });

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "IsActive" },
                values: new object[] { new DateTime(2024, 9, 30, 6, 59, 8, 496, DateTimeKind.Utc).AddTicks(3592), true });

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "IsActive" },
                values: new object[] { new DateTime(2024, 9, 30, 6, 59, 8, 496, DateTimeKind.Utc).AddTicks(3593), true });

            migrationBuilder.InsertData(
                table: "ProductOptions",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Period", "PeriodDescription", "Price", "ProductId" },
                values: new object[,]
                {
                    { 4, new DateTime(2024, 9, 30, 6, 59, 8, 496, DateTimeKind.Utc).AddTicks(3594), false, 7, "7 Days", 0m, 1 },
                    { 5, new DateTime(2024, 9, 30, 6, 59, 8, 496, DateTimeKind.Utc).AddTicks(3596), false, 15, "15 Days", 0m, 1 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductKeys_OrderItems_OrderItemId",
                table: "ProductKeys",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductKeys_Users_UserId",
                table: "ProductKeys",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductKeys_OrderItems_OrderItemId",
                table: "ProductKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductKeys_Users_UserId",
                table: "ProductKeys");

            migrationBuilder.DeleteData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProductOptions");

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "ProductKeys",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "OrderItemId",
                table: "ProductKeys",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 22, 23, 54, 25, 256, DateTimeKind.Utc).AddTicks(612));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 22, 23, 54, 25, 256, DateTimeKind.Utc).AddTicks(620));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 22, 23, 54, 25, 256, DateTimeKind.Utc).AddTicks(622));

            migrationBuilder.AddForeignKey(
                name: "FK_ProductKeys_OrderItems_OrderItemId",
                table: "ProductKeys",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductKeys_Users_UserId",
                table: "ProductKeys",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
