using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pilgrims.PersonalFinances.Migrations
{
    /// <inheritdoc />
    public partial class AddGoalModelAndEnhancedLinking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    GoalType = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TargetDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    LinkedAccountId = table.Column<string>(type: "TEXT", nullable: true),
                    LinkedBudgetId = table.Column<string>(type: "TEXT", nullable: true),
                    LinkedDebtId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_Accounts_LinkedAccountId",
                        column: x => x.LinkedAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Goals_Budgets_LinkedBudgetId",
                        column: x => x.LinkedBudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Goals_Debts_LinkedDebtId",
                        column: x => x.LinkedDebtId,
                        principalTable: "Debts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_GoalType",
                table: "Goals",
                column: "GoalType");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_IsActive",
                table: "Goals",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_LinkedAccountId",
                table: "Goals",
                column: "LinkedAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_LinkedBudgetId",
                table: "Goals",
                column: "LinkedBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_LinkedDebtId",
                table: "Goals",
                column: "LinkedDebtId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_TargetDate",
                table: "Goals",
                column: "TargetDate");

            // Add new columns to existing Notification table for enhanced linking
            migrationBuilder.AddColumn<string>(
                name: "LinkedGoalId",
                table: "Notifications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedAccountId",
                table: "Notifications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedDebtId",
                table: "Notifications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SnoozedUntil",
                table: "Notifications",
                type: "TEXT",
                nullable: true);

            // Create foreign key constraints for the new notification linking columns
            migrationBuilder.CreateIndex(
                name: "IX_Notifications_LinkedGoalId",
                table: "Notifications",
                column: "LinkedGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_LinkedAccountId",
                table: "Notifications",
                column: "LinkedAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_LinkedDebtId",
                table: "Notifications",
                column: "LinkedDebtId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Goals_LinkedGoalId",
                table: "Notifications",
                column: "LinkedGoalId",
                principalTable: "Goals",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Accounts_LinkedAccountId",
                table: "Notifications",
                column: "LinkedAccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Debts_LinkedDebtId",
                table: "Notifications",
                column: "LinkedDebtId",
                principalTable: "Debts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Goals_LinkedGoalId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Accounts_LinkedAccountId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Debts_LinkedDebtId",
                table: "Notifications");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_Notifications_LinkedGoalId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_LinkedAccountId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_LinkedDebtId",
                table: "Notifications");

            // Drop columns from Notifications table
            migrationBuilder.DropColumn(
                name: "LinkedGoalId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "LinkedAccountId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "LinkedDebtId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SnoozedUntil",
                table: "Notifications");

            // Drop Goals table
            migrationBuilder.DropTable(
                name: "Goals");
        }
    }
}