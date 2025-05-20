using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wiseshare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class recalculateValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TotalInvestmentAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    TotalReturns = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                    table.UniqueConstraint("AK_Portfolios_UserId", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OriginalValue = table.Column<double>(type: "REAL", precision: 18, scale: 2, nullable: false),
                    CurrentValue = table.Column<double>(type: "REAL", precision: 18, scale: 2, nullable: false),
                    SharePrice = table.Column<double>(type: "REAL", precision: 18, scale: 2, nullable: false),
                    AvailableShares = table.Column<int>(type: "INTEGER", precision: 18, scale: 2, nullable: false, defaultValue: 20000),
                    Name = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    ImageUrls = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    InvestmentsEnabled = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "User"),
                    SecurityQuestion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SecurityAnswer = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", maxLength: 6, nullable: false, defaultValue: true),
                    IsEmailVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailVerifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Balance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Investments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PortfolioId = table.Column<string>(type: "TEXT", nullable: false),
                    NumOfSharesPurchased = table.Column<int>(type: "INTEGER", nullable: false),
                    InvestmentAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DivedendEarned = table.Column<float>(type: "REAL", precision: 18, scale: 2, nullable: false, defaultValue: 0f),
                    IsSellPending = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    PendingSharesToSell = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    MarketValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Investments_Portfolios_UserId",
                        column: x => x.UserId,
                        principalTable: "Portfolios",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Investments_UserId",
                table: "Investments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Address",
                table: "Properties",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Name",
                table: "Properties",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Investments");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Portfolios");
        }
    }
}
