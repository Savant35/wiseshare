using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WiseShare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class property1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AvailableShares",
                table: "Properties",
                type: "INTEGER",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 20000,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Name_Address",
                table: "Properties",
                columns: new[] { "Name", "Address" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Properties_Name_Address",
                table: "Properties");

            migrationBuilder.AlterColumn<int>(
                name: "AvailableShares",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 20000);
        }
    }
}
