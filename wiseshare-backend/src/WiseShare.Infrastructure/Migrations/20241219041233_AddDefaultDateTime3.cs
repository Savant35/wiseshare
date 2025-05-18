using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WiseShare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultDateTime3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime('now', 'localtime')",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime('now', 'localtime')",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDateTime",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "datetime('now', 'localtime')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "datetime('now', 'localtime')");
        }
    }
}
