using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pilgrims.PersonalFinances.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Transactions_PurchaseTransactionId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationHistory_Accounts_AccountId",
                table: "NotificationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationHistory_Budgets_BudgetId",
                table: "NotificationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationHistory_Debts_DebtId",
                table: "NotificationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationHistory_ScheduledTransactions_ScheduledTransactionId",
                table: "NotificationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationHistory_Transactions_TransactionId",
                table: "NotificationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UserId",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Transactions",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoalId",
                table: "Transactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoalId1",
                table: "Transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SnoozedUntil",
                table: "TransactionNotifications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DebtId",
                table: "ScheduledTransactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ReconciliationSessions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ReconciliationItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "NotificationHistory",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "MaintenanceRecords",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Investments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Debts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Creditors",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Budgets",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoalId",
                table: "Budgets",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoalId1",
                table: "Budgets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemplate",
                table: "Budgets",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BudgetId1",
                table: "BudgetCategories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BudgetAlerts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Severity",
                table: "BudgetAlerts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ThresholdPercentage",
                table: "BudgetAlerts",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentValue",
                table: "Assets",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "AssetCategoryId",
                table: "Assets",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Assets",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Assets",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "AssetRegisters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AssetRegisters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextMaintenanceDate",
                table: "AssetMaintenances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetId",
                table: "AssetDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetRegisterId1",
                table: "AssetDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "Accounts",
                type: "TEXT",
                maxLength: 7,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Accounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    EnableReminders = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReminderFrequencyDays = table.Column<int>(type: "INTEGER", nullable: false),
                    LastReminderDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Insurances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    PolicyNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    InsuranceCompany = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PolicyType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PolicyName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AgentName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    AgentEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    AgentPhone = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CoverageAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PremiumAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PremiumFrequency = table.Column<int>(type: "INTEGER", nullable: false),
                    PolicyStartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PolicyEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextPremiumDueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeductibleAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    BeneficiaryName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BeneficiaryRelationship = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BeneficiaryPhone = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    BeneficiaryEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insurances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Obligations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    OrganizationName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MembershipNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ContributionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OriginalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinimumPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ContributionFrequency = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextContributionDueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastPaymentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LateFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GracePeriodDays = table.Column<int>(type: "INTEGER", nullable: true),
                    TermInMonths = table.Column<int>(type: "INTEGER", nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AvailableCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsCompoundInterest = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContactPhone = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    ContactEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ContactAddress = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    ContactPerson = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Website = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpectedAnnualReturn = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BenefitsDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    LastDividendDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastDividendAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obligations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReadDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DismissedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SnoozedUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRecurring = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecurrenceType = table.Column<int>(type: "INTEGER", nullable: true),
                    RecurrenceInterval = table.Column<int>(type: "INTEGER", nullable: true),
                    RecurrenceEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    BudgetId = table.Column<string>(type: "TEXT", nullable: true),
                    DebtId = table.Column<string>(type: "TEXT", nullable: true),
                    GoalId = table.Column<string>(type: "TEXT", nullable: true),
                    ScheduledTransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    ActionData = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ActionUrl = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    EnableSound = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableVibration = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdvanceNoticeDays = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxSnoozeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentSnoozeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notification_Debts_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notification_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notification_ScheduledTransactions_ScheduledTransactionId",
                        column: x => x.ScheduledTransactionId,
                        principalTable: "ScheduledTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notification_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceBeneficiaries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    InsuranceId = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Relationship = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IdentificationNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceBeneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceBeneficiaries_Insurances_InsuranceId",
                        column: x => x.InsuranceId,
                        principalTable: "Insurances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceClaims",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    InsuranceId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ClaimDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    IncidentLocation = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ClaimAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    SettlementDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceClaims_Insurances_InsuranceId",
                        column: x => x.InsuranceId,
                        principalTable: "Insurances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsurancePremiumPayments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    InsuranceId = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PaymentStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TransactionReference = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    LateFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LateFeeAppliedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextDueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePremiumPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsurancePremiumPayments_Insurances_InsuranceId",
                        column: x => x.InsuranceId,
                        principalTable: "Insurances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObligationBenefits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ObligationId = table.Column<string>(type: "TEXT", nullable: false),
                    BenefitType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProcessedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationBenefits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObligationBenefits_Obligations_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObligationDocuments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ObligationId = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObligationDocuments_Obligations_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObligationPayments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ObligationId = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PaymentStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TransactionReference = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    LateFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LateFeeAppliedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PaymentPeriod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InterestAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RemainingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsExtraPayment = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObligationPayments_Obligations_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceDocuments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    InsuranceId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimId = table.Column<string>(type: "TEXT", nullable: true),
                    DocumentName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceDocuments_InsuranceClaims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "InsuranceClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InsuranceDocuments_Insurances_InsuranceId",
                        column: x => x.InsuranceId,
                        principalTable: "Insurances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    InsuranceId = table.Column<string>(type: "TEXT", nullable: false),
                    InsuranceClaimId = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    NotificationType = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReadDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDismissed = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdditionalData = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceNotifications_InsuranceClaims_InsuranceClaimId",
                        column: x => x.InsuranceClaimId,
                        principalTable: "InsuranceClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InsuranceNotifications_Insurances_InsuranceId",
                        column: x => x.InsuranceId,
                        principalTable: "Insurances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObligationNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ObligationId = table.Column<string>(type: "TEXT", nullable: false),
                    ObligationPaymentId = table.Column<string>(type: "TEXT", nullable: true),
                    ObligationBenefitId = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    NotificationType = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReadDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDismissed = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdditionalData = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObligationNotifications_ObligationBenefits_ObligationBenefitId",
                        column: x => x.ObligationBenefitId,
                        principalTable: "ObligationBenefits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ObligationNotifications_ObligationPayments_ObligationPaymentId",
                        column: x => x.ObligationPaymentId,
                        principalTable: "ObligationPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ObligationNotifications_Obligations_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GoalId1",
                table: "Transactions",
                column: "GoalId1");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTransactions_DebtId",
                table: "ScheduledTransactions",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_GoalId1",
                table: "Budgets",
                column: "GoalId1");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_BudgetId1",
                table: "BudgetCategories",
                column: "BudgetId1");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDocuments_AssetId",
                table: "AssetDocuments",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDocuments_AssetRegisterId1",
                table: "AssetDocuments",
                column: "AssetRegisterId1");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_GoalType",
                table: "Goals",
                column: "GoalType");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_IsActive",
                table: "Goals",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_TargetDate",
                table: "Goals",
                column: "TargetDate");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceBeneficiaries_FullName",
                table: "InsuranceBeneficiaries",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceBeneficiaries_InsuranceId",
                table: "InsuranceBeneficiaries",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceBeneficiaries_IsActive",
                table: "InsuranceBeneficiaries",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_ClaimDate",
                table: "InsuranceClaims",
                column: "ClaimDate");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_ClaimNumber",
                table: "InsuranceClaims",
                column: "ClaimNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_InsuranceId",
                table: "InsuranceClaims",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_Status",
                table: "InsuranceClaims",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceDocuments_ClaimId",
                table: "InsuranceDocuments",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceDocuments_DocumentType",
                table: "InsuranceDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceDocuments_InsuranceId",
                table: "InsuranceDocuments",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceDocuments_UploadDate",
                table: "InsuranceDocuments",
                column: "UploadDate");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceNotifications_InsuranceClaimId",
                table: "InsuranceNotifications",
                column: "InsuranceClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceNotifications_InsuranceId",
                table: "InsuranceNotifications",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceNotifications_IsSent",
                table: "InsuranceNotifications",
                column: "IsSent");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceNotifications_ScheduledDate",
                table: "InsuranceNotifications",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePremiumPayments_InsuranceId",
                table: "InsurancePremiumPayments",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_Insurances_PolicyEndDate",
                table: "Insurances",
                column: "PolicyEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Insurances_PolicyNumber",
                table: "Insurances",
                column: "PolicyNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Insurances_Status",
                table: "Insurances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_BudgetId",
                table: "Notification",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_DebtId",
                table: "Notification",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_GoalId",
                table: "Notification",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ScheduledDate",
                table: "Notification",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ScheduledTransactionId",
                table: "Notification",
                column: "ScheduledTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Status",
                table: "Notification",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TransactionId",
                table: "Notification",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Type",
                table: "Notification",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationBenefits_ObligationId",
                table: "ObligationBenefits",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationBenefits_ReceivedDate",
                table: "ObligationBenefits",
                column: "ReceivedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationBenefits_Status",
                table: "ObligationBenefits",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationDocuments_ObligationId",
                table: "ObligationDocuments",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationNotifications_IsSent",
                table: "ObligationNotifications",
                column: "IsSent");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationNotifications_ObligationBenefitId",
                table: "ObligationNotifications",
                column: "ObligationBenefitId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationNotifications_ObligationId",
                table: "ObligationNotifications",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationNotifications_ObligationPaymentId",
                table: "ObligationNotifications",
                column: "ObligationPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationNotifications_ScheduledDate",
                table: "ObligationNotifications",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationPayments_ObligationId_PaymentDate",
                table: "ObligationPayments",
                columns: new[] { "ObligationId", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ObligationPayments_PaymentDate",
                table: "ObligationPayments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Obligations_Name",
                table: "Obligations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Obligations_Status",
                table: "Obligations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Obligations_Type",
                table: "Obligations",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDocuments_AssetRegisters_AssetRegisterId1",
                table: "AssetDocuments",
                column: "AssetRegisterId1",
                principalTable: "AssetRegisters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDocuments_Assets_AssetId",
                table: "AssetDocuments",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Transactions_PurchaseTransactionId",
                table: "Assets",
                column: "PurchaseTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetCategories_Budgets_BudgetId1",
                table: "BudgetCategories",
                column: "BudgetId1",
                principalTable: "Budgets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_Goals_GoalId1",
                table: "Budgets",
                column: "GoalId1",
                principalTable: "Goals",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationHistory_Accounts_AccountId",
                table: "NotificationHistory",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationHistory_Budgets_BudgetId",
                table: "NotificationHistory",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationHistory_Debts_DebtId",
                table: "NotificationHistory",
                column: "DebtId",
                principalTable: "Debts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationHistory_ScheduledTransactions_ScheduledTransactionId",
                table: "NotificationHistory",
                column: "ScheduledTransactionId",
                principalTable: "ScheduledTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationHistory_Transactions_TransactionId",
                table: "NotificationHistory",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledTransactions_Debts_DebtId",
                table: "ScheduledTransactions",
                column: "DebtId",
                principalTable: "Debts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Goals_GoalId1",
                table: "Transactions",
                column: "GoalId1",
                principalTable: "Goals",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UserId",
                table: "Transactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetDocuments_AssetRegisters_AssetRegisterId1",
                table: "AssetDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetDocuments_Assets_AssetId",
                table: "AssetDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Transactions_PurchaseTransactionId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetCategories_Budgets_BudgetId1",
                table: "BudgetCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Goals_GoalId1",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationHistory_Accounts_AccountId",
                table: "NotificationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationHistory_Budgets_BudgetId",
                table: "NotificationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationHistory_Debts_DebtId",
                table: "NotificationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationHistory_ScheduledTransactions_ScheduledTransactionId",
                table: "NotificationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationHistory_Transactions_TransactionId",
                table: "NotificationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledTransactions_Debts_DebtId",
                table: "ScheduledTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Goals_GoalId1",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UserId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "InsuranceBeneficiaries");

            migrationBuilder.DropTable(
                name: "InsuranceDocuments");

            migrationBuilder.DropTable(
                name: "InsuranceNotifications");

            migrationBuilder.DropTable(
                name: "InsurancePremiumPayments");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "ObligationDocuments");

            migrationBuilder.DropTable(
                name: "ObligationNotifications");

            migrationBuilder.DropTable(
                name: "InsuranceClaims");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "ObligationBenefits");

            migrationBuilder.DropTable(
                name: "ObligationPayments");

            migrationBuilder.DropTable(
                name: "Insurances");

            migrationBuilder.DropTable(
                name: "Obligations");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_GoalId1",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_ScheduledTransactions_DebtId",
                table: "ScheduledTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_GoalId1",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_BudgetCategories_BudgetId1",
                table: "BudgetCategories");

            migrationBuilder.DropIndex(
                name: "IX_AssetDocuments_AssetId",
                table: "AssetDocuments");

            migrationBuilder.DropIndex(
                name: "IX_AssetDocuments_AssetRegisterId1",
                table: "AssetDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "GoalId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "GoalId1",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SnoozedUntil",
                table: "TransactionNotifications");

            migrationBuilder.DropColumn(
                name: "DebtId",
                table: "ScheduledTransactions");

            migrationBuilder.DropColumn(
                name: "GoalId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "GoalId1",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "IsTemplate",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "BudgetId1",
                table: "BudgetCategories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BudgetAlerts");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "BudgetAlerts");

            migrationBuilder.DropColumn(
                name: "ThresholdPercentage",
                table: "BudgetAlerts");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "AssetRegisters");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AssetRegisters");

            migrationBuilder.DropColumn(
                name: "NextMaintenanceDate",
                table: "AssetMaintenances");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "AssetDocuments");

            migrationBuilder.DropColumn(
                name: "AssetRegisterId1",
                table: "AssetDocuments");

            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Accounts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ReconciliationSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ReconciliationItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "NotificationHistory",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "MaintenanceRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Investments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Debts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Creditors",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Budgets",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentValue",
                table: "Assets",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssetCategoryId",
                table: "Assets",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Accounts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "1",
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 21, 2, 41, 57, 282, DateTimeKind.Utc).AddTicks(339));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "2",
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 21, 2, 41, 57, 283, DateTimeKind.Utc).AddTicks(720));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "3",
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 21, 2, 41, 57, 283, DateTimeKind.Utc).AddTicks(732));

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Transactions_PurchaseTransactionId",
                table: "Assets",
                column: "PurchaseTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationHistory_Accounts_AccountId",
                table: "NotificationHistory",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationHistory_Budgets_BudgetId",
                table: "NotificationHistory",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationHistory_Debts_DebtId",
                table: "NotificationHistory",
                column: "DebtId",
                principalTable: "Debts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationHistory_ScheduledTransactions_ScheduledTransactionId",
                table: "NotificationHistory",
                column: "ScheduledTransactionId",
                principalTable: "ScheduledTransactions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationHistory_Transactions_TransactionId",
                table: "NotificationHistory",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UserId",
                table: "Transactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
