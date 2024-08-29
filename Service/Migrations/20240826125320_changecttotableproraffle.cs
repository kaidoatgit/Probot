using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProPayments.Service.Migrations
{
    /// <inheritdoc />
    public partial class changecttotableproraffle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "UserSettings");

            migrationBuilder.AddColumn<Guid>(
                name: "Version",
                table: "ProRaffles",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"))
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 26, 12, 53, 19, 997, DateTimeKind.Utc).AddTicks(3645));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 26, 12, 53, 19, 997, DateTimeKind.Utc).AddTicks(3657));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 26, 12, 53, 19, 997, DateTimeKind.Utc).AddTicks(3669));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "ProRaffles");

            migrationBuilder.AddColumn<Guid>(
                name: "Version",
                table: "UserSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"))
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 26, 10, 57, 59, 241, DateTimeKind.Utc).AddTicks(9340));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 26, 10, 57, 59, 241, DateTimeKind.Utc).AddTicks(9352));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 26, 10, 57, 59, 241, DateTimeKind.Utc).AddTicks(9355));
        }
    }
}
