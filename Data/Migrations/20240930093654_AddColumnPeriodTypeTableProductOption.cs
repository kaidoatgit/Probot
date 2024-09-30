using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Probot.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnPeriodTypeTableProductOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductOptions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<string>(
                name: "PeriodDescription",
                table: "ProductOptions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "ProductOptions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER")
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AddColumn<string>(
                name: "PeriodType",
                table: "ProductOptions",
                type: "TEXT",
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PeriodType", "Price" },
                values: new object[] { new DateTime(2024, 9, 30, 9, 36, 54, 328, DateTimeKind.Utc).AddTicks(8772), "Month", 10m });

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PeriodType", "Price" },
                values: new object[] { new DateTime(2024, 9, 30, 9, 36, 54, 328, DateTimeKind.Utc).AddTicks(8780), "Month", 19m });

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PeriodType", "Price" },
                values: new object[] { new DateTime(2024, 9, 30, 9, 36, 54, 328, DateTimeKind.Utc).AddTicks(8781), "Month", 28m });

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PeriodType" },
                values: new object[] { new DateTime(2024, 9, 30, 9, 36, 54, 328, DateTimeKind.Utc).AddTicks(8783), "Day" });

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PeriodType" },
                values: new object[] { new DateTime(2024, 9, 30, 9, 36, 54, 328, DateTimeKind.Utc).AddTicks(8785), "Day" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodType",
                table: "ProductOptions");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductOptions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<string>(
                name: "PeriodDescription",
                table: "ProductOptions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Relational:ColumnOrder", 4)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "ProductOptions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Price" },
                values: new object[] { new DateTime(2024, 9, 30, 6, 59, 8, 496, DateTimeKind.Utc).AddTicks(3583), 12m });

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Price" },
                values: new object[] { new DateTime(2024, 9, 30, 6, 59, 8, 496, DateTimeKind.Utc).AddTicks(3592), 20m });

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Price" },
                values: new object[] { new DateTime(2024, 9, 30, 6, 59, 8, 496, DateTimeKind.Utc).AddTicks(3593), 30m });

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 30, 6, 59, 8, 496, DateTimeKind.Utc).AddTicks(3594));

            migrationBuilder.UpdateData(
                table: "ProductOptions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2024, 9, 30, 6, 59, 8, 496, DateTimeKind.Utc).AddTicks(3596));
        }
    }
}
