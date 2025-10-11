using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pilgrims.PersonalFinances.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultCurrency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    DateFormat = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NumberFormat = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Theme = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ShowCurrencyCode = table.Column<bool>(type: "INTEGER", nullable: false),
                    CurrencyDecimalPlaces = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdatedVersion = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    LastModifiedByUserId = table.Column<int>(type: "INTEGER", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "1",
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 29, 10, 28, 5, 507, DateTimeKind.Utc).AddTicks(4869));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "2",
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 29, 10, 28, 5, 507, DateTimeKind.Utc).AddTicks(9349));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "3",
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 29, 10, 28, 5, 507, DateTimeKind.Utc).AddTicks(9365));

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSettings_IsActive",
                table: "ApplicationSettings",
                column: "IsActive");

            // Insert default application settings
            migrationBuilder.InsertData(
                table: "ApplicationSettings",
                columns: new[] { "Id", "DefaultCurrency", "DateFormat", "NumberFormat", "Theme", "ShowCurrencyCode", "CurrencyDecimalPlaces", "LastUpdatedVersion", "LastModifiedByUserId", "IsActive", "CreatedAt", "UpdatedAt", "IsDirty" },
                values: new object[] { "default-settings", "KES", "MM/dd/yyyy", "en-US", "Light", false, 2, "1.0.0", null, true, new DateTime(2025, 9, 29, 10, 28, 5, 507, DateTimeKind.Utc), null, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApplicationSettings",
                keyColumn: "Id",
                keyValue: "default-settings");

            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "1",
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 29, 5, 52, 3, 768, DateTimeKind.Utc).AddTicks(1159));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "2",
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 29, 5, 52, 3, 768, DateTimeKind.Utc).AddTicks(4321));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "3",
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 29, 5, 52, 3, 768, DateTimeKind.Utc).AddTicks(4329));
        }
    }
}
