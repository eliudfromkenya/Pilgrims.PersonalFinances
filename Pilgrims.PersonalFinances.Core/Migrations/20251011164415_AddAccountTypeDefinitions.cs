using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pilgrims.PersonalFinances.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountTypeDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationSettings_IsActive",
                table: "ApplicationSettings");

            migrationBuilder.CreateTable(
                name: "AccountTypeDefinitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EnumValue = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypeDefinitions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AccountTypeDefinitions",
                columns: new[] { "Id", "CreatedAt", "Description", "EnumValue", "Icon", "IsActive", "IsDirty", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { "acc-type-1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A standard checking account for daily transactions.", 1, "🏦", true, false, "Checking Account", null },
                    { "acc-type-2", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A savings account typically used for saving money.", 2, "💰", true, false, "Savings Account", null },
                    { "acc-type-3", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Represents cash held physically or in simple cash accounts.", 3, "💵", true, false, "Cash Account", null },
                    { "acc-type-4", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A credit card account used for purchases.", 4, "💳", true, false, "Credit Card", null },
                    { "acc-type-5", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "An investment account for stocks, bonds, and other assets.", 5, "📈", true, false, "Investment Account", null },
                    { "acc-type-6", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "An account representing loans with balances owed.", 6, "🏠", true, false, "Loan Account", null },
                    { "acc-type-7", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A general credit account representing lines of credit.", 7, "💳", true, false, "Credit Account", null },
                    { "acc-type-8", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Other types of accounts not categorized above.", 8, "📋", true, false, "Other", null }
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "1",
                column: "UpdatedAt",
                value: new DateTime(2025, 10, 11, 16, 44, 12, 755, DateTimeKind.Utc).AddTicks(1087));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "2",
                column: "UpdatedAt",
                value: new DateTime(2025, 10, 11, 16, 44, 12, 755, DateTimeKind.Utc).AddTicks(2205));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "3",
                column: "UpdatedAt",
                value: new DateTime(2025, 10, 11, 16, 44, 12, 755, DateTimeKind.Utc).AddTicks(2221));

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypeDefinitions_EnumValue",
                table: "AccountTypeDefinitions",
                column: "EnumValue",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypeDefinitions_IsActive",
                table: "AccountTypeDefinitions",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountTypeDefinitions");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "1",
                column: "UpdatedAt",
                value: new DateTime(2025, 10, 11, 14, 1, 53, 918, DateTimeKind.Utc).AddTicks(3992));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "2",
                column: "UpdatedAt",
                value: new DateTime(2025, 10, 11, 14, 1, 53, 918, DateTimeKind.Utc).AddTicks(9000));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "3",
                column: "UpdatedAt",
                value: new DateTime(2025, 10, 11, 14, 1, 53, 918, DateTimeKind.Utc).AddTicks(9010));

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSettings_IsActive",
                table: "ApplicationSettings",
                column: "IsActive");
        }
    }
}
