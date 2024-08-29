using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProPayments.Service.Migrations
{
    /// <inheritdoc />
    public partial class RefactorConcurrencyTokenTableProRaffles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_UserSettings_UserSettingId",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<ulong>(
                name: "UserSettingId",
                table: "Subscriptions",
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
                value: new DateTime(2024, 8, 26, 9, 50, 9, 98, DateTimeKind.Utc).AddTicks(7377));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 26, 9, 50, 9, 98, DateTimeKind.Utc).AddTicks(7388));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 26, 9, 50, 9, 98, DateTimeKind.Utc).AddTicks(7390));

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_UserSettings_UserSettingId",
                table: "Subscriptions",
                column: "UserSettingId",
                principalTable: "UserSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_UserSettings_UserSettingId",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<ulong>(
                name: "UserSettingId",
                table: "Subscriptions",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 24, 19, 20, 59, 369, DateTimeKind.Utc).AddTicks(1353));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 24, 19, 20, 59, 369, DateTimeKind.Utc).AddTicks(1363));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 8, 24, 19, 20, 59, 369, DateTimeKind.Utc).AddTicks(1365));

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_UserSettings_UserSettingId",
                table: "Subscriptions",
                column: "UserSettingId",
                principalTable: "UserSettings",
                principalColumn: "Id");
        }
    }
}
