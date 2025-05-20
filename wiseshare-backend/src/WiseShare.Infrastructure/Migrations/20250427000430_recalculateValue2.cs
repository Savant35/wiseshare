using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wiseshare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class recalculateValue2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalReturns",
                table: "Portfolios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalReturns",
                table: "Portfolios",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
