using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Probot.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnRegisterTableProRaffleSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrationAlertToken",
                table: "ProRaffleSettings",
                newName: "RegisterAlertToken");

            migrationBuilder.RenameColumn(
                name: "RegistrationAlertId",
                table: "ProRaffleSettings",
                newName: "RegisterAlertId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegisterAlertToken",
                table: "ProRaffleSettings",
                newName: "RegistrationAlertToken");

            migrationBuilder.RenameColumn(
                name: "RegisterAlertId",
                table: "ProRaffleSettings",
                newName: "RegistrationAlertId");

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 20, 16, 22, 22, 632, DateTimeKind.Utc).AddTicks(2970));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 20, 16, 22, 22, 632, DateTimeKind.Utc).AddTicks(2980));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 20, 16, 22, 22, 632, DateTimeKind.Utc).AddTicks(2983));
        }
    }
}
