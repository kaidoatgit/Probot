using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Probot.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamingColumnsProRaffleSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegisteredRafflesToken",
                table: "ProRaffleSettings",
                newName: "RegistrationAlertToken");

            migrationBuilder.RenameColumn(
                name: "RegisteredRafflesId",
                table: "ProRaffleSettings",
                newName: "RegistrationAlertId");

            migrationBuilder.RenameColumn(
                name: "IsRegisteredRafflesNotificationEnabled",
                table: "ProRaffleSettings",
                newName: "IsRegisteredAlertEnabled");

            migrationBuilder.RenameColumn(
                name: "IsFailedRafflesNotificationEnabled",
                table: "ProRaffleSettings",
                newName: "IsErrorAlertEnabled");

            migrationBuilder.RenameColumn(
                name: "FailedRafflesToken",
                table: "ProRaffleSettings",
                newName: "ErrorAlertToken");

            migrationBuilder.RenameColumn(
                name: "FailedRafflesId",
                table: "ProRaffleSettings",
                newName: "ErrorAlertId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrationAlertToken",
                table: "ProRaffleSettings",
                newName: "RegisteredRafflesToken");

            migrationBuilder.RenameColumn(
                name: "RegistrationAlertId",
                table: "ProRaffleSettings",
                newName: "RegisteredRafflesId");

            migrationBuilder.RenameColumn(
                name: "IsRegisteredAlertEnabled",
                table: "ProRaffleSettings",
                newName: "IsRegisteredRafflesNotificationEnabled");

            migrationBuilder.RenameColumn(
                name: "IsErrorAlertEnabled",
                table: "ProRaffleSettings",
                newName: "IsFailedRafflesNotificationEnabled");

            migrationBuilder.RenameColumn(
                name: "ErrorAlertToken",
                table: "ProRaffleSettings",
                newName: "FailedRafflesToken");

            migrationBuilder.RenameColumn(
                name: "ErrorAlertId",
                table: "ProRaffleSettings",
                newName: "FailedRafflesId");

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
    }
}
