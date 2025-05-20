using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wiseshare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class recalculateValue3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RealizedProfit",
                table: "Portfolios",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RealizedProfit",
                table: "Portfolios");
        }
    }
}
