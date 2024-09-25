using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Probot.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProRaffleNotificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailedRafflesId",
                table: "ProRaffleSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FailedRafflesToken",
                table: "ProRaffleSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFailedRafflesNotificationEnabled",
                table: "ProRaffleSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRegisteredRafflesNotificationEnabled",
                table: "ProRaffleSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredRafflesId",
                table: "ProRaffleSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegisteredRafflesToken",
                table: "ProRaffleSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedRafflesId",
                table: "ProRaffleSettings");

            migrationBuilder.DropColumn(
                name: "FailedRafflesToken",
                table: "ProRaffleSettings");

            migrationBuilder.DropColumn(
                name: "IsFailedRafflesNotificationEnabled",
                table: "ProRaffleSettings");

            migrationBuilder.DropColumn(
                name: "IsRegisteredRafflesNotificationEnabled",
                table: "ProRaffleSettings");

            migrationBuilder.DropColumn(
                name: "RegisteredRafflesId",
                table: "ProRaffleSettings");

            migrationBuilder.DropColumn(
                name: "RegisteredRafflesToken",
                table: "ProRaffleSettings");

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 7, 13, 50, 8, 482, DateTimeKind.Utc).AddTicks(4903));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 7, 13, 50, 8, 482, DateTimeKind.Utc).AddTicks(4914));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 7, 13, 50, 8, 482, DateTimeKind.Utc).AddTicks(4916));
        }
    }
}
