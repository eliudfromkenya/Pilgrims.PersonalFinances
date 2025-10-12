using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pilgrims.PersonalFinances.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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

            migrationBuilder.CreateTable(
                name: "AssetCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 7, nullable: true),
                    DefaultDepreciationMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DefaultDepreciationRate = table.Column<decimal>(type: "TEXT", nullable: true),
                    DefaultUsefulLifeYears = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Operation = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    EntityName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    OldValues = table.Column<string>(type: "TEXT", nullable: true),
                    NewValues = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Creditors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CustomerServicePhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    AccountNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creditors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CurrencyName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ISOCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    SymbolOrSign = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

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
                name: "IncomeCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeCategories", x => x.Id);
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
                name: "NotificationSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    NotificationType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    DaysInAdvance = table.Column<int>(type: "INTEGER", nullable: false),
                    HoursInAdvance = table.Column<int>(type: "INTEGER", nullable: false),
                    PreferredChannels = table.Column<int>(type: "INTEGER", nullable: false),
                    PreferredTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    AllowSnooze = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultSnoozeDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxSnoozeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    BudgetThresholdPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    BudgetAlertType = table.Column<int>(type: "INTEGER", nullable: true),
                    BudgetThresholdAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SendOnWeekends = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendOnHolidays = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuietHoursStart = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    QuietHoursEnd = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    CustomMessageTemplate = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
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
                name: "ReportTemplates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ReportType = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultChartType = table.Column<int>(type: "INTEGER", nullable: false),
                    Configuration = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultDateRange = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultComparisonPeriod = table.Column<int>(type: "INTEGER", nullable: false),
                    IsSystemTemplate = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ColorTheme = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    RequiredRole = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UsageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    PasswordSalt = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    TimeZone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    LockedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmailVerificationToken = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    EmailVerificationTokenExpiry = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProfilePicturePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Preferences = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditEntries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AuditLogId = table.Column<string>(type: "TEXT", nullable: false),
                    PropertyName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "TEXT", nullable: true),
                    NewValue = table.Column<string>(type: "TEXT", nullable: true),
                    DataType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsSensitive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditEntries_AuditLogs_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AuditLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Debts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DebtType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreditorId = table.Column<string>(type: "TEXT", nullable: true),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 4, nullable: true),
                    MinimumPayment = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PaymentDueDay = table.Column<int>(type: "INTEGER", nullable: true),
                    PaymentFrequency = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MaturityDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AccountNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    PaidOffDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Debts_Creditors_CreditorId",
                        column: x => x.CreditorId,
                        principalTable: "Creditors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Incomes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IncomeType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Frequency = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsRegular = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPreTax = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextExpectedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IncomeCategoryId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incomes_IncomeCategories_IncomeCategoryId",
                        column: x => x.IncomeCategoryId,
                        principalTable: "IncomeCategories",
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
                name: "ReportParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ReportTemplateId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ParameterType = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowMultiple = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ValidationRules = table.Column<string>(type: "TEXT", nullable: true),
                    Options = table.Column<string>(type: "TEXT", nullable: true),
                    MinValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    MaxValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    MinDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MaxDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Pattern = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MinLength = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxLength = table.Column<int>(type: "INTEGER", nullable: true),
                    ValidationPattern = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Placeholder = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CssClass = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    Group = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ConditionalRules = table.Column<string>(type: "TEXT", nullable: true),
                    ReportTemplateId1 = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportParameters_ReportTemplates_ReportTemplateId",
                        column: x => x.ReportTemplateId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportParameters_ReportTemplates_ReportTemplateId1",
                        column: x => x.ReportTemplateId1,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ReportTemplateId = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ReportType = table.Column<int>(type: "INTEGER", nullable: false),
                    ChartType = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateRangePreset = table.Column<int>(type: "INTEGER", nullable: true),
                    ComparisonPeriod = table.Column<int>(type: "INTEGER", nullable: false),
                    Parameters = table.Column<string>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    ChartData = table.Column<string>(type: "TEXT", nullable: true),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ExportPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ExportFormat = table.Column<int>(type: "INTEGER", nullable: true),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: true),
                    RecordCount = table.Column<int>(type: "INTEGER", nullable: false),
                    GenerationTimeMs = table.Column<int>(type: "INTEGER", nullable: true),
                    IsScheduled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ScheduleFrequency = table.Column<int>(type: "INTEGER", nullable: true),
                    NextRunTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsShared = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastViewedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReportTemplateId1 = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_ReportTemplates_ReportTemplateId",
                        column: x => x.ReportTemplateId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_ReportTemplates_ReportTemplateId1",
                        column: x => x.ReportTemplateId1,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AccountType = table.Column<int>(type: "INTEGER", nullable: false),
                    InitialBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ColorCode = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    BankName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(5,4)", nullable: true),
                    StatementDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MinimumPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BrokerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AccountHolder = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastReconciledDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReconciledBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ColorCode = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    IconName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentCategoryId = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    SessionToken = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DeviceInfo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastAccessedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RevocationReason = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
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

            migrationBuilder.CreateTable(
                name: "Investments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    InvestmentType = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountId = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Investments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReconciliationSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AccountId = table.Column<string>(type: "TEXT", nullable: false),
                    SessionName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ReconciliationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StatementStartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StatementEndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StatementStartingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StatementEndingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BookStartingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BookEndingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Difference = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    BankStatementFilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsReconciled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReconciledDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReconciledBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconciliationSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReconciliationSessions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BudgetType = table.Column<int>(type: "INTEGER", nullable: false),
                    Period = table.Column<int>(type: "INTEGER", nullable: false),
                    LimitAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SpentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowOverspend = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableRollover = table.Column<bool>(type: "INTEGER", nullable: false),
                    RolloverAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoryId = table.Column<string>(type: "TEXT", nullable: true),
                    AccountId = table.Column<string>(type: "TEXT", nullable: true),
                    Tag = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    GoalId = table.Column<int>(type: "INTEGER", nullable: true),
                    AlertLevels = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AlertsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsTemplate = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastAlertLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    GoalId1 = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budgets_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Budgets_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Budgets_Goals_GoalId1",
                        column: x => x.GoalId1,
                        principalTable: "Goals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Budgets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionType = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountId = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<string>(type: "TEXT", nullable: true),
                    TransferToAccountId = table.Column<string>(type: "TEXT", nullable: true),
                    Payee = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    RecurrenceType = table.Column<int>(type: "INTEGER", nullable: false),
                    RecurrenceInterval = table.Column<int>(type: "INTEGER", nullable: false),
                    DaysOfWeek = table.Column<int>(type: "INTEGER", nullable: true),
                    DayOfMonth = table.Column<int>(type: "INTEGER", nullable: true),
                    MonthOfYear = table.Column<int>(type: "INTEGER", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastProcessedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndType = table.Column<int>(type: "INTEGER", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MaxOccurrences = table.Column<int>(type: "INTEGER", nullable: true),
                    CurrentOccurrences = table.Column<int>(type: "INTEGER", nullable: false),
                    SchedulingMode = table.Column<int>(type: "INTEGER", nullable: false),
                    NotificationTiming = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastGeneratedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextDueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SkippedDates = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    AdjustForWeekends = table.Column<bool>(type: "INTEGER", nullable: false),
                    DebtId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledTransactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledTransactions_Accounts_TransferToAccountId",
                        column: x => x.TransferToAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduledTransactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScheduledTransactions_Debts_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BudgetAlerts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    BudgetId = table.Column<string>(type: "TEXT", nullable: false),
                    AlertLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    BudgetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SpentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UsedPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReadDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Severity = table.Column<int>(type: "INTEGER", nullable: false),
                    ThresholdPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetAlerts_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    BudgetId = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<string>(type: "TEXT", nullable: true),
                    AccountId = table.Column<string>(type: "TEXT", nullable: true),
                    TagName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AllocatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SpentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BudgetId1 = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetCategories_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetCategories_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetCategories_Budgets_BudgetId1",
                        column: x => x.BudgetId1,
                        principalTable: "Budgets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BudgetCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetDebt",
                columns: table => new
                {
                    RelatedBudgetsId = table.Column<string>(type: "TEXT", nullable: false),
                    RelatedDebtsId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetDebt", x => new { x.RelatedBudgetsId, x.RelatedDebtsId });
                    table.ForeignKey(
                        name: "FK_BudgetDebt_Budgets_RelatedBudgetsId",
                        column: x => x.RelatedBudgetsId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetDebt_Debts_RelatedDebtsId",
                        column: x => x.RelatedDebtsId,
                        principalTable: "Debts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebtPayments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DebtId = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InterestAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FeesAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsScheduled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ScheduledTransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtPayments_Debts_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebtPayments_ScheduledTransactions_ScheduledTransactionId",
                        column: x => x.ScheduledTransactionId,
                        principalTable: "ScheduledTransactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NotificationRules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    NotificationType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Frequency = table.Column<int>(type: "INTEGER", nullable: false),
                    NotificationSettingsId = table.Column<string>(type: "TEXT", nullable: false),
                    ScheduledTransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    BudgetId = table.Column<string>(type: "TEXT", nullable: true),
                    DebtId = table.Column<string>(type: "TEXT", nullable: true),
                    AccountId = table.Column<string>(type: "TEXT", nullable: true),
                    CategoryId = table.Column<string>(type: "TEXT", nullable: true),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DayOfMonth = table.Column<int>(type: "INTEGER", nullable: true),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomCondition = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    LastEvaluated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastTriggered = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TriggerCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxTriggers = table.Column<int>(type: "INTEGER", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationRules_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationRules_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationRules_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationRules_Debts_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationRules_NotificationSettings_NotificationSettingsId",
                        column: x => x.NotificationSettingsId,
                        principalTable: "NotificationSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationRules_ScheduledTransactions_ScheduledTransactionId",
                        column: x => x.ScheduledTransactionId,
                        principalTable: "ScheduledTransactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TransactionNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ScheduledTransactionId = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    NotificationType = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDismissed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    AdditionalData = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxRetries = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SnoozedUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionNotifications_ScheduledTransactions_ScheduledTransactionId",
                        column: x => x.ScheduledTransactionId,
                        principalTable: "ScheduledTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetDocuments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AssetRegisterId = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DocumentType = table.Column<int>(type: "INTEGER", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    UploadDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    AssetId = table.Column<string>(type: "TEXT", nullable: true),
                    AssetRegisterId1 = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetInsurances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AssetId = table.Column<string>(type: "TEXT", nullable: false),
                    PolicyNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    InsuranceProvider = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AgentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AgentContact = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CoverageAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Deductible = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CoverageType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CoverageDetails = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    PolicyStartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PolicyEndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RenewalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AnnualPremium = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyPremium = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PaymentFrequency = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    AutoRenewal = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PrimaryBeneficiary = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    SecondaryBeneficiary = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ClaimsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalClaimsAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LastClaimDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PolicyDocumentPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CertificatePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ScheduledTransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetInsurances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetInsurances_ScheduledTransactions_ScheduledTransactionId",
                        column: x => x.ScheduledTransactionId,
                        principalTable: "ScheduledTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AssetMaintenances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AssetId = table.Column<string>(type: "TEXT", nullable: false),
                    ServiceType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ServiceDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextMaintenanceDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ServiceProvider = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ServiceProviderContact = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MileageAtService = table.Column<int>(type: "INTEGER", nullable: true),
                    HoursAtService = table.Column<int>(type: "INTEGER", nullable: true),
                    NextServiceDue = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextServiceMileage = table.Column<int>(type: "INTEGER", nullable: true),
                    NextServiceHours = table.Column<int>(type: "INTEGER", nullable: true),
                    ServiceCategory = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    WarrantyExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    WarrantyMileage = table.Column<int>(type: "INTEGER", nullable: true),
                    WarrantyDetails = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ReceiptPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ServiceReportPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    PartsCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LaborCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetMaintenances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetRegisters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AssetId = table.Column<string>(type: "TEXT", nullable: false),
                    PurchaseReceiptPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    Vendor = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PurchaseOrderNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    WarrantyDocumentPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    WarrantyStartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    WarrantyEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    WarrantyProvider = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    WarrantyTerms = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    InsurancePolicyPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    InsurancePolicyNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    InsuranceProvider = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    InsuranceStartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InsuranceEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InsuranceCoverage = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    InsurancePremium = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    LastMaintenanceDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextMaintenanceDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MaintenanceSchedule = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MaintenanceNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ManualPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CertificatesPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AdditionalDocumentsPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModelNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ResponsiblePerson = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetRegisters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AssetRegisterId = table.Column<string>(type: "TEXT", nullable: false),
                    MaintenanceType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    MaintenanceDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextMaintenanceDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ServiceProvider = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ServiceProviderContact = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DocumentPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsRecurring = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecurrenceIntervalDays = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceRecords_AssetRegisters_AssetRegisterId",
                        column: x => x.AssetRegisterId,
                        principalTable: "AssetRegisters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AssetCategoryId = table.Column<string>(type: "TEXT", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DepreciationMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DepreciationRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    UsefulLifeYears = table.Column<int>(type: "INTEGER", nullable: true),
                    SalvageValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Brand = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PurchaseTransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisposalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DisposalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReceiptPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    WarrantyPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    InsuranceDocumentPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Condition = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AssetRegisterId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AssetCategories_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalTable: "AssetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountId = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<string>(type: "TEXT", nullable: true),
                    TransferToAccountId = table.Column<string>(type: "TEXT", nullable: true),
                    Payee = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ClearedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReconciledDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsRecurring = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecurringPattern = table.Column<string>(type: "TEXT", nullable: true),
                    IsSplit = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentTransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    BudgetId = table.Column<string>(type: "TEXT", nullable: true),
                    DebtId = table.Column<string>(type: "TEXT", nullable: true),
                    AssetId = table.Column<string>(type: "TEXT", nullable: true),
                    GoalId = table.Column<int>(type: "INTEGER", nullable: true),
                    GoalId1 = table.Column<string>(type: "TEXT", nullable: true),
                    InvestmentId = table.Column<string>(type: "TEXT", nullable: true),
                    ScheduledTransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_TransferToAccountId",
                        column: x => x.TransferToAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_Debts_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Goals_GoalId1",
                        column: x => x.GoalId1,
                        principalTable: "Goals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Investments_InvestmentId",
                        column: x => x.InvestmentId,
                        principalTable: "Investments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_ScheduledTransactions_ScheduledTransactionId",
                        column: x => x.ScheduledTransactionId,
                        principalTable: "ScheduledTransactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Transactions_ParentTransactionId",
                        column: x => x.ParentTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
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
                name: "NotificationHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    NotificationRuleId = table.Column<string>(type: "TEXT", nullable: true),
                    NotificationType = table.Column<int>(type: "INTEGER", nullable: false),
                    Channel = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Data = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DismissedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SnoozeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastSnoozedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SnoozeUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActionTaken = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ActionTakenAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActionResult = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    RetryAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    ScheduledTransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    BudgetId = table.Column<string>(type: "TEXT", nullable: true),
                    DebtId = table.Column<string>(type: "TEXT", nullable: true),
                    AccountId = table.Column<string>(type: "TEXT", nullable: true),
                    NotificationRuleId1 = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationHistory_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NotificationHistory_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NotificationHistory_Debts_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NotificationHistory_NotificationRules_NotificationRuleId",
                        column: x => x.NotificationRuleId,
                        principalTable: "NotificationRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NotificationHistory_NotificationRules_NotificationRuleId1",
                        column: x => x.NotificationRuleId1,
                        principalTable: "NotificationRules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationHistory_ScheduledTransactions_ScheduledTransactionId",
                        column: x => x.ScheduledTransactionId,
                        principalTable: "ScheduledTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NotificationHistory_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ReconciliationItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ReconciliationSessionId = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ItemType = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsCleared = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClearedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsStatementOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsBookOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconciliationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReconciliationItems_ReconciliationSessions_ReconciliationSessionId",
                        column: x => x.ReconciliationSessionId,
                        principalTable: "ReconciliationSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReconciliationItems_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SplitTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionId = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoryId = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Memo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SplitTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SplitTransactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SplitTransactions_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionAttachments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionId = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UploadedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionAttachments_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "ColorCode", "CreatedAt", "Description", "IconName", "IsActive", "IsDirty", "Name", "ParentCategoryId", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { "1", "#FF6B6B", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, true, "Debt Payment", null, new DateTime(2025, 10, 11, 14, 1, 53, 918, DateTimeKind.Utc).AddTicks(3992), null },
                    { "2", "#FF4757", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, true, "Interest", null, new DateTime(2025, 10, 11, 14, 1, 53, 918, DateTimeKind.Utc).AddTicks(9000), null },
                    { "3", "#FF3838", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, true, "Credit Card", null, new DateTime(2025, 10, 11, 14, 1, 53, 918, DateTimeKind.Utc).AddTicks(9010), null }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Country", "CreatedAt", "CurrencyName", "ISOCode", "IsDirty", "Notes", "SymbolOrSign", "UpdatedAt" },
                values: new object[,]
                {
                    { "1", "Afghanistan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Afghani", "AFN", false, "Independent currency", "؋", null },
                    { "10", "Austria", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "100", "Liechtenstein", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Swiss Franc", "CHF", false, "Uses foreign currency", "Fr", null },
                    { "101", "Lithuania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "102", "Luxembourg", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "103", "Madagascar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Malagasy Ariary", "MGA", false, "Independent currency", "Ar", null },
                    { "104", "Malawi", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Malawian Kwacha", "MWK", false, "Independent currency", "MK", null },
                    { "105", "Malaysia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Malaysian Ringgit", "MYR", false, "Independent currency", "RM", null },
                    { "106", "Maldives", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Maldivian Rufiyaa", "MVR", false, "Independent currency", "Rf", null },
                    { "107", "Mali", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "West African CFA Franc", "XOF", false, "Shared currency (WAEMU)", "Fr", null },
                    { "108", "Malta", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "109", "Marshall Islands", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United States Dollar", "USD", false, "Uses foreign currency", "$", null },
                    { "11", "Azerbaijan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Azerbaijani Manat", "AZN", false, "Independent currency", "₼", null },
                    { "110", "Mauritania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mauritanian Ouguiya", "MRU", false, "Independent currency", "UM", null },
                    { "111", "Mauritius", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mauritian Rupee", "MUR", false, "Independent currency", "₨", null },
                    { "112", "Mexico", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mexican Peso", "MXN", false, "Independent currency", "$", null },
                    { "113", "Micronesia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United States Dollar", "USD", false, "Uses foreign currency", "$", null },
                    { "114", "Moldova", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Moldovan Leu", "MDL", false, "Independent currency", "L", null },
                    { "115", "Monaco", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "116", "Mongolia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mongolian Tögrög", "MNT", false, "Independent currency", "₮", null },
                    { "117", "Montenegro", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (not official EU member)", "€", null },
                    { "118", "Morocco", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Moroccan Dirham", "MAD", false, "Independent currency", "د.م", null },
                    { "119", "Mozambique", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mozambican Metical", "MZN", false, "Independent currency", "MT", null },
                    { "12", "Bahamas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bahamian Dollar", "BSD", false, "Pegged 1:1 USD", "$", null },
                    { "120", "Myanmar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Burmese Kyat", "MMK", false, "Independent currency", "K", null },
                    { "121", "Namibia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Namibian Dollar", "NAD", false, "Pegged to South African Rand", "$", null },
                    { "122", "Nauru", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Australian Dollar", "AUD", false, "Uses foreign currency", "$", null },
                    { "123", "Nepal", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nepalese Rupee", "NPR", false, "Pegged to Indian Rupee", "₨", null },
                    { "124", "Netherlands", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "125", "New Zealand", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "New Zealand Dollar", "NZD", false, "Independent currency", "$", null },
                    { "126", "Nicaragua", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nicaraguan Córdoba", "NIO", false, "Independent currency", "C$", null },
                    { "127", "Niger", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "West African CFA Franc", "XOF", false, "Shared currency (WAEMU)", "Fr", null },
                    { "128", "Nigeria", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nigerian Naira", "NGN", false, "Independent currency", "₦", null },
                    { "129", "North Macedonia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Macedonian Denar", "MKD", false, "Independent currency", "ден", null },
                    { "13", "Bahrain", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bahraini Dinar", "BHD", false, "Pegged to USD", ".د.ب", null },
                    { "130", "Norway", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Norwegian Krone", "NOK", false, "Independent currency", "kr", null },
                    { "131", "Oman", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Omani Rial", "OMR", false, "Pegged to USD", "ر.ع.", null },
                    { "132", "Pakistan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pakistani Rupee", "PKR", false, "Independent currency", "₨", null },
                    { "133", "Palau", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United States Dollar", "USD", false, "Uses foreign currency", "$", null },
                    { "134", "Panama", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Panamanian Balboa", "PAB", false, "Pegged to USD (also uses USD)", "B/.", null },
                    { "135", "Papua New Guinea", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Papua New Guinean Kina", "PGK", false, "Independent currency", "K", null },
                    { "136", "Paraguay", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Paraguayan Guaraní", "PYG", false, "Independent currency", "₲", null },
                    { "137", "Peru", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Peruvian Sol", "PEN", false, "Independent currency", "S/", null },
                    { "138", "Philippines", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Philippine Peso", "PHP", false, "Independent currency", "₱", null },
                    { "139", "Poland", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Polish Zloty", "PLN", false, "Independent currency", "zł", null },
                    { "14", "Bangladesh", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Taka", "BDT", false, "Independent currency", "৳", null },
                    { "140", "Portugal", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "141", "Qatar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Qatari Riyal", "QAR", false, "Pegged to USD", "ر.ق", null },
                    { "142", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Romanian Leu", "RON", false, "Independent currency", "lei", null },
                    { "143", "Russia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Russian Ruble", "RUB", false, "Independent currency", "₽", null },
                    { "144", "Rwanda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rwandan Franc", "RWF", false, "Independent currency", "FRw", null },
                    { "145", "Saint Kitts and Nevis", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "East Caribbean Dollar", "XCD", false, "Shared currency (Eastern Caribbean)", "$", null },
                    { "146", "Saint Lucia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "East Caribbean Dollar", "XCD", false, "Shared currency (Eastern Caribbean)", "$", null },
                    { "147", "Saint Vincent and the Grenadines", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "East Caribbean Dollar", "XCD", false, "Shared currency (Eastern Caribbean)", "$", null },
                    { "148", "Samoa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Samoan Tala", "WST", false, "Independent currency", "WS$", null },
                    { "149", "San Marino", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "15", "Barbados", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Barbadian Dollar", "BBD", false, "Pegged 2:1 USD", "$", null },
                    { "150", "Saudi Arabia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Saudi Riyal", "SAR", false, "Pegged to USD", "ر.س", null },
                    { "151", "Senegal", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "West African CFA Franc", "XOF", false, "Shared currency (WAEMU)", "CFA", null },
                    { "152", "Serbia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Serbian Dinar", "RSD", false, "Independent currency", "din", null },
                    { "153", "Seychelles", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seychellois Rupee", "SCR", false, "Independent currency", "₨", null },
                    { "154", "Sierra Leone", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sierra Leonean Leone", "SLL", false, "Independent currency", "Le", null },
                    { "155", "Singapore", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Singapore Dollar", "SGD", false, "Independent currency", "$", null },
                    { "156", "Slovakia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "157", "Slovenia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "158", "Solomon Islands", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Solomon Islands Dollar", "SBD", false, "Independent currency", "SI$", null },
                    { "159", "Somalia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Somali Shilling", "SOS", false, "Independent currency", "Sh", null },
                    { "16", "Belarus", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Belarusian Ruble", "BYN", false, "Independent currency", "Br", null },
                    { "160", "South Africa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "South African Rand", "ZAR", false, "Independent currency", "R", null },
                    { "161", "South Korea", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "South Korean Won", "KRW", false, "Independent currency", "₩", null },
                    { "162", "South Sudan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "South Sudanese Pound", "SSP", false, "Independent currency", "£", null },
                    { "163", "Spain", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "164", "Sri Lanka", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sri Lankan Rupee", "LKR", false, "Independent currency", "₨", null },
                    { "165", "Sudan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sudanese Pound", "SDG", false, "Independent currency", "ج.س", null },
                    { "166", "Suriname", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Surinamese Dollar", "SRD", false, "Independent currency", "$", null },
                    { "167", "Sweden", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Swedish Krona", "SEK", false, "Independent currency", "kr", null },
                    { "168", "Switzerland", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Swiss Franc", "CHF", false, "Independent currency", "Fr", null },
                    { "169", "Syria", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Syrian Pound", "SYP", false, "Independent currency", "£", null },
                    { "17", "Belgium", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "170", "Taiwan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "New Taiwan Dollar", "TWD", false, "Independent currency", "NT$", null },
                    { "171", "Tajikistan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tajikistani Somoni", "TJS", false, "Independent currency", "SM", null },
                    { "172", "Tanzania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tanzanian Shilling", "TZS", false, "Independent currency", "Tsh", null },
                    { "173", "Thailand", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thai Baht", "THB", false, "Independent currency", "฿", null },
                    { "174", "Timor-Leste", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United States Dollar", "USD", false, "Uses foreign currency", "$", null },
                    { "175", "Togo", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "West African CFA Franc", "XOF", false, "Shared currency (WAEMU)", "CFA", null },
                    { "176", "Tonga", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tongan Paʻanga", "TOP", false, "Independent currency", "T$", null },
                    { "177", "Trinidad and Tobago", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trinidad and Tobago Dollar", "TTD", false, "Independent currency", "TT$", null },
                    { "178", "Tunisia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tunisian Dinar", "TND", false, "Independent currency", "د.ت", null },
                    { "179", "Turkey", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Turkish Lira", "TRY", false, "Independent currency", "₺", null },
                    { "18", "Belize", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Belize Dollar", "BZD", false, "Pegged 2:1 USD", "$", null },
                    { "180", "Turkmenistan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Turkmenistani Manat", "TMT", false, "Independent currency", "m", null },
                    { "181", "Tuvalu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Australian Dollar", "AUD", false, "Uses foreign currency", "$", null },
                    { "182", "Uganda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ugandan Shilling", "UGX", false, "Independent currency", "Ush", null },
                    { "183", "Ukraine", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ukrainian Hryvnia", "UAH", false, "Independent currency", "₴", null },
                    { "184", "United Arab Emirates", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "UAE Dirham", "AED", false, "Pegged to USD", "د.إ", null },
                    { "185", "United Kingdom", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "British Pound Sterling", "GBP", false, "Independent currency", "£", null },
                    { "186", "United States", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United States Dollar", "USD", false, "World's primary reserve currency", "$", null },
                    { "187", "Uruguay", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Uruguayan Peso", "UYU", false, "Independent currency", "$", null },
                    { "188", "Uzbekistan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Uzbekistani Som", "UZS", false, "Independent currency", "so'm", null },
                    { "189", "Vanuatu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vanuatu Vatu", "VUV", false, "Independent currency", "Vt", null },
                    { "19", "Benin", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "West African CFA Franc", "XOF", false, "Shared currency (West African Union)", "Fr", null },
                    { "190", "Vatican City", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Vatican agreement)", "€", null },
                    { "191", "Venezuela", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Venezuelan Bolívar", "VES", false, "Independent currency", "Bs.S", null },
                    { "192", "Vietnam", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vietnamese Dong", "VND", false, "Independent currency", "₫", null },
                    { "193", "Yemen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Yemeni Rial", "YER", false, "Independent currency", "﷼", null },
                    { "194", "Zambia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Zambian Kwacha", "ZMW", false, "Independent currency", "ZK", null },
                    { "195", "Zimbabwe", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United States Dollar", "USD", false, "Uses foreign currency (multi-currency system)", "$", null },
                    { "2", "Albania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lek", "ALL", false, "Independent currency", "L", null },
                    { "20", "Bhutan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ngultrum", "BTN", false, "Pegged 1:1 INR", "Nu.", null },
                    { "21", "Bolivia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Boliviano", "BOB", false, "Independent currency", "Bs.", null },
                    { "22", "Bosnia & Herzegovina", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Convertible Mark", "BAM", false, "Pegged to EUR", "KM", null },
                    { "23", "Botswana", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pula", "BWP", false, "Independent currency", "P", null },
                    { "24", "Brazil", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Brazilian Real", "BRL", false, "Independent currency", "R$", null },
                    { "25", "Brunei", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Brunei Dollar", "BND", false, "Pegged to SGD", "$", null },
                    { "26", "Bulgaria", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bulgarian Lev", "BGN", false, "Independent currency", "лв", null },
                    { "27", "Burkina Faso", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "West African CFA Franc", "XOF", false, "Shared currency (West African Union)", "Fr", null },
                    { "28", "Burundi", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Burundian Franc", "BIF", false, "Independent currency", "Fr", null },
                    { "29", "Cambodia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Riel", "KHR", false, "Independent currency", "៛", null },
                    { "3", "Algeria", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Algerian Dinar", "DZD", false, "Independent currency", "دج", null },
                    { "30", "Cameroon", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Central African CFA Franc", "XAF", false, "Shared currency (Central African Union)", "Fr", null },
                    { "31", "Canada", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Canadian Dollar", "CAD", false, "Independent currency", "$", null },
                    { "32", "Cape Verde", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cape Verdean Escudo", "CVE", false, "Pegged to EUR", "$", null },
                    { "33", "Central African Republic", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Central African CFA Franc", "XAF", false, "Shared currency (Central African Union)", "Fr", null },
                    { "34", "Chad", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Central African CFA Franc", "XAF", false, "Shared currency (Central African Union)", "Fr", null },
                    { "35", "Chile", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chilean Peso", "CLP", false, "Independent currency", "$", null },
                    { "36", "China", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Renminbi (Yuan)", "CNY", false, "Independent currency", "¥", null },
                    { "37", "Colombia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Colombian Peso", "COP", false, "Independent currency", "$", null },
                    { "38", "Comoros", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Comorian Franc", "KMF", false, "Pegged to EUR", "Fr", null },
                    { "39", "Congo (DRC)", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Congolese Franc", "CDF", false, "Independent currency", "Fr", null },
                    { "4", "Andorra", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (no national currency)", "€", null },
                    { "40", "Congo (Republic)", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Central African CFA Franc", "XAF", false, "Shared currency (Central African Union)", "Fr", null },
                    { "41", "Costa Rica", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Costa Rican Colón", "CRC", false, "Independent currency", "₡", null },
                    { "42", "Croatia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Adopted EUR in 2023", "€", null },
                    { "43", "Cuba", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cuban Peso", "CUP", false, "Independent currency", "$", null },
                    { "44", "Cyprus", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "45", "Czech Republic", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Czech Koruna", "CZK", false, "Independent currency", "Kč", null },
                    { "46", "Denmark", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Danish Krone", "DKK", false, "Independent currency", "kr", null },
                    { "47", "Djibouti", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Djiboutian Franc", "DJF", false, "Pegged to USD", "Fr", null },
                    { "48", "Dominica", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "East Caribbean Dollar", "XCD", false, "Shared currency (Eastern Caribbean)", "$", null },
                    { "49", "Dominican Republic", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dominican Peso", "DOP", false, "Independent currency", "$", null },
                    { "5", "Angola", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kwanza", "AOA", false, "Independent currency", "Kz", null },
                    { "50", "Ecuador", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "US Dollar", "USD", false, "Dollarized economy", "$", null },
                    { "51", "Egypt", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Egyptian Pound", "EGP", false, "Independent currency", "£ / ج.م", null },
                    { "52", "El Salvador", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "US Dollar / Bitcoin", "USD / BTC", false, "Officially USD + BTC", "$ / ₿", null },
                    { "53", "Equatorial Guinea", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Central African CFA Franc", "XAF", false, "Shared currency (Central African Union)", "Fr", null },
                    { "54", "Eritrea", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nakfa", "ERN", false, "Independent currency", "Nfk", null },
                    { "55", "Estonia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "56", "Eswatini (Swaziland)", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lilangeni", "SZL", false, "Pegged to ZAR", "E", null },
                    { "57", "Ethiopia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ethiopian Birr", "ETB", false, "Independent currency", "Br", null },
                    { "58", "Fiji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fijian Dollar", "FJD", false, "Independent currency", "$", null },
                    { "59", "Finland", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "6", "Antigua & Barbuda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "East Caribbean Dollar", "XCD", false, "Shared currency (Eastern Caribbean)", "$", null },
                    { "60", "France", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "61", "Gabon", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Central African CFA Franc", "XAF", false, "Shared currency (Central African Union)", "Fr", null },
                    { "62", "Gambia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gambian Dalasi", "GMD", false, "Independent currency", "D", null },
                    { "63", "Georgia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Georgian Lari", "GEL", false, "Independent currency", "₾", null },
                    { "64", "Germany", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "65", "Ghana", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ghanaian Cedi", "GHS", false, "Independent currency", "₵", null },
                    { "66", "Greece", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "67", "Grenada", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "East Caribbean Dollar", "XCD", false, "Shared currency (Eastern Caribbean)", "$", null },
                    { "68", "Guatemala", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Guatemalan Quetzal", "GTQ", false, "Independent currency", "Q", null },
                    { "69", "Guinea", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Guinean Franc", "GNF", false, "Independent currency", "Fr", null },
                    { "7", "Argentina", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Argentine Peso", "ARS", false, "Independent currency", "$", null },
                    { "70", "Guinea-Bissau", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "West African CFA Franc", "XOF", false, "Shared currency (West African Union)", "Fr", null },
                    { "71", "Guyana", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Guyanese Dollar", "GYD", false, "Independent currency", "$", null },
                    { "72", "Haiti", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Haitian Gourde", "HTG", false, "Independent currency", "G", null },
                    { "73", "Honduras", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Honduran Lempira", "HNL", false, "Independent currency", "L", null },
                    { "74", "Hungary", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hungarian Forint", "HUF", false, "Independent currency", "Ft", null },
                    { "75", "Iceland", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Icelandic Krona", "ISK", false, "Independent currency", "kr", null },
                    { "76", "India", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Indian Rupee", "INR", false, "Independent currency", "₹", null },
                    { "77", "Indonesia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Indonesian Rupiah", "IDR", false, "Independent currency", "Rp", null },
                    { "78", "Iran", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Iranian Rial", "IRR", false, "Independent currency", "﷼", null },
                    { "79", "Iraq", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Iraqi Dinar", "IQD", false, "Independent currency", "ع.د", null },
                    { "8", "Armenia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Armenian Dram", "AMD", false, "Independent currency", "֏", null },
                    { "80", "Ireland", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "81", "Israel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Israeli New Shekel", "ILS", false, "Independent currency", "₪", null },
                    { "82", "Italy", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "83", "Ivory Coast (Côte d'Ivoire)", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "West African CFA Franc", "XOF", false, "Shared currency (West African Union)", "Fr", null },
                    { "84", "Jamaica", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Jamaican Dollar", "JMD", false, "Independent currency", "$", null },
                    { "85", "Japan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Japanese Yen", "JPY", false, "Independent currency", "¥", null },
                    { "86", "Jordan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Jordanian Dinar", "JOD", false, "Pegged to USD", "د.ا", null },
                    { "87", "Kazakhstan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kazakhstani Tenge", "KZT", false, "Independent currency", "₸", null },
                    { "88", "Kenya", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kenyan Shilling", "KES", false, "Independent currency", "Ksh", null },
                    { "89", "Kiribati", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Australian Dollar", "AUD", false, "Uses foreign currency", "$", null },
                    { "9", "Australia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Australian Dollar", "AUD", false, "Independent currency", "$", null },
                    { "90", "Korea, North", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "North Korean Won", "KPW", false, "Independent currency", "₩", null },
                    { "91", "Korea, South", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "South Korean Won", "KRW", false, "Independent currency", "₩", null },
                    { "92", "Kuwait", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kuwaiti Dinar", "KWD", false, "One of the strongest currencies", "د.ك", null },
                    { "93", "Kyrgyzstan", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kyrgyzstani Som", "KGS", false, "Independent currency", "с", null },
                    { "94", "Laos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lao Kip", "LAK", false, "Independent currency", "₭", null },
                    { "95", "Latvia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "EUR", false, "Uses EUR (Eurozone member)", "€", null },
                    { "96", "Lebanon", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lebanese Pound", "LBP", false, "Pegged/managed", "ل.ل", null },
                    { "97", "Lesotho", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lesotho Loti", "LSL", false, "Pegged to South African Rand", "L", null },
                    { "98", "Liberia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Liberian Dollar", "LRD", false, "Independent currency", "$", null },
                    { "99", "Libya", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Libyan Dinar", "LYD", false, "Independent currency", "ل.د", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountNumber",
                table: "Accounts",
                column: "AccountNumber",
                unique: true,
                filter: "[AccountNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountType",
                table: "Accounts",
                column: "AccountType");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSettings_IsActive",
                table: "ApplicationSettings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_Name",
                table: "AssetCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetDocuments_AssetId",
                table: "AssetDocuments",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDocuments_AssetRegisterId",
                table: "AssetDocuments",
                column: "AssetRegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDocuments_AssetRegisterId1",
                table: "AssetDocuments",
                column: "AssetRegisterId1");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInsurances_AssetId",
                table: "AssetInsurances",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInsurances_IsActive",
                table: "AssetInsurances",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInsurances_PolicyEndDate",
                table: "AssetInsurances",
                column: "PolicyEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInsurances_PolicyNumber",
                table: "AssetInsurances",
                column: "PolicyNumber");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInsurances_ScheduledTransactionId",
                table: "AssetInsurances",
                column: "ScheduledTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetMaintenances_AssetId",
                table: "AssetMaintenances",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetMaintenances_NextServiceDue",
                table: "AssetMaintenances",
                column: "NextServiceDue");

            migrationBuilder.CreateIndex(
                name: "IX_AssetMaintenances_ServiceDate",
                table: "AssetMaintenances",
                column: "ServiceDate");

            migrationBuilder.CreateIndex(
                name: "IX_AssetMaintenances_Status",
                table: "AssetMaintenances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AssetMaintenances_TransactionId",
                table: "AssetMaintenances",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRegisters_AssetId",
                table: "AssetRegisters",
                column: "AssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetRegisters_PurchaseDate",
                table: "AssetRegisters",
                column: "PurchaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRegisters_SerialNumber",
                table: "AssetRegisters",
                column: "SerialNumber",
                unique: true,
                filter: "[SerialNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetCategoryId",
                table: "Assets",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_PurchaseDate",
                table: "Assets",
                column: "PurchaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_PurchaseTransactionId",
                table: "Assets",
                column: "PurchaseTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SerialNumber",
                table: "Assets",
                column: "SerialNumber",
                unique: true,
                filter: "[SerialNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEntries_AuditLogId",
                table: "AuditEntries",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAlerts_BudgetId",
                table: "BudgetAlerts",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_AccountId",
                table: "BudgetCategories",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_BudgetId_CategoryId",
                table: "BudgetCategories",
                columns: new[] { "BudgetId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_BudgetId1",
                table: "BudgetCategories",
                column: "BudgetId1");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_CategoryId",
                table: "BudgetCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetDebt_RelatedDebtsId",
                table: "BudgetDebt",
                column: "RelatedDebtsId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_AccountId",
                table: "Budgets",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_CategoryId",
                table: "Budgets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_EndDate",
                table: "Budgets",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_GoalId1",
                table: "Budgets",
                column: "GoalId1");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_IsActive",
                table: "Budgets",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_StartDate",
                table: "Budgets",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserId",
                table: "Budgets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Creditors_Name",
                table: "Creditors",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebtPayments_DebtId_PaymentDate",
                table: "DebtPayments",
                columns: new[] { "DebtId", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DebtPayments_PaymentDate",
                table: "DebtPayments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_DebtPayments_ScheduledTransactionId",
                table: "DebtPayments",
                column: "ScheduledTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Debts_CreditorId_AccountNumber",
                table: "Debts",
                columns: new[] { "CreditorId", "AccountNumber" },
                unique: true,
                filter: "[AccountNumber] IS NOT NULL");

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
                name: "IX_IncomeCategories_Name",
                table: "IncomeCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_IncomeCategoryId",
                table: "Incomes",
                column: "IncomeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_IncomeType",
                table: "Incomes",
                column: "IncomeType");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_IsRegular",
                table: "Incomes",
                column: "IsRegular");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_ReceivedDate",
                table: "Incomes",
                column: "ReceivedDate");

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
                name: "IX_Investments_AccountId",
                table: "Investments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Investments_PurchaseDate",
                table: "Investments",
                column: "PurchaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Investments_Symbol",
                table: "Investments",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_AssetRegisterId",
                table: "MaintenanceRecords",
                column: "AssetRegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_MaintenanceDate",
                table: "MaintenanceRecords",
                column: "MaintenanceDate");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_NextMaintenanceDate",
                table: "MaintenanceRecords",
                column: "NextMaintenanceDate");

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
                name: "IX_NotificationHistory_AccountId",
                table: "NotificationHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_BudgetId",
                table: "NotificationHistory",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_DebtId",
                table: "NotificationHistory",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_NotificationRuleId",
                table: "NotificationHistory",
                column: "NotificationRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_NotificationRuleId1",
                table: "NotificationHistory",
                column: "NotificationRuleId1");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_ReadAt",
                table: "NotificationHistory",
                column: "ReadAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_ScheduledAt",
                table: "NotificationHistory",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_ScheduledTransactionId",
                table: "NotificationHistory",
                column: "ScheduledTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_SentAt",
                table: "NotificationHistory",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_Status",
                table: "NotificationHistory",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_TransactionId",
                table: "NotificationHistory",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRules_AccountId",
                table: "NotificationRules",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRules_BudgetId",
                table: "NotificationRules",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRules_CategoryId",
                table: "NotificationRules",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRules_DebtId",
                table: "NotificationRules",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRules_IsActive",
                table: "NotificationRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRules_NotificationSettingsId",
                table: "NotificationRules",
                column: "NotificationSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRules_NotificationType",
                table: "NotificationRules",
                column: "NotificationType");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRules_ScheduledTransactionId",
                table: "NotificationRules",
                column: "ScheduledTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_NotificationType",
                table: "NotificationSettings",
                column: "NotificationType");

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

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationItems_ReconciliationSessionId",
                table: "ReconciliationItems",
                column: "ReconciliationSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationItems_Status",
                table: "ReconciliationItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationItems_TransactionDate",
                table: "ReconciliationItems",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationItems_TransactionId",
                table: "ReconciliationItems",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationSessions_AccountId",
                table: "ReconciliationSessions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationSessions_ReconciliationDate",
                table: "ReconciliationSessions",
                column: "ReconciliationDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationSessions_Status",
                table: "ReconciliationSessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParameters_Name",
                table: "ReportParameters",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParameters_ParameterType",
                table: "ReportParameters",
                column: "ParameterType");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParameters_ReportTemplateId",
                table: "ReportParameters",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParameters_ReportTemplateId1",
                table: "ReportParameters",
                column: "ReportTemplateId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CreatedAt",
                table: "Reports",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportTemplateId",
                table: "Reports",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportTemplateId1",
                table: "Reports",
                column: "ReportTemplateId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportType",
                table: "Reports",
                column: "ReportType");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_Status",
                table: "Reports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_IsSystemTemplate",
                table: "ReportTemplates",
                column: "IsSystemTemplate");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_Name",
                table: "ReportTemplates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_ReportType",
                table: "ReportTemplates",
                column: "ReportType");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTransactions_AccountId",
                table: "ScheduledTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTransactions_CategoryId",
                table: "ScheduledTransactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTransactions_DebtId",
                table: "ScheduledTransactions",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTransactions_IsActive",
                table: "ScheduledTransactions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTransactions_NextDueDate",
                table: "ScheduledTransactions",
                column: "NextDueDate");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTransactions_TransferToAccountId",
                table: "ScheduledTransactions",
                column: "TransferToAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitTransactions_CategoryId",
                table: "SplitTransactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitTransactions_TransactionId",
                table: "SplitTransactions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionAttachments_TransactionId",
                table: "TransactionAttachments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionNotifications_IsRead",
                table: "TransactionNotifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionNotifications_ScheduledDate",
                table: "TransactionNotifications",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionNotifications_ScheduledTransactionId",
                table: "TransactionNotifications",
                column: "ScheduledTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AssetId",
                table: "Transactions",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BudgetId",
                table: "Transactions",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Date",
                table: "Transactions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DebtId",
                table: "Transactions",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GoalId1",
                table: "Transactions",
                column: "GoalId1");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_InvestmentId",
                table: "Transactions",
                column: "InvestmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ParentTransactionId",
                table: "Transactions",
                column: "ParentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ScheduledTransactionId",
                table: "Transactions",
                column: "ScheduledTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransferToAccountId",
                table: "Transactions",
                column: "TransferToAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailVerificationToken",
                table: "Users",
                column: "EmailVerificationToken");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PasswordResetToken",
                table: "Users",
                column: "PasswordResetToken");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_ExpiresAt",
                table: "UserSessions",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_RefreshToken",
                table: "UserSessions",
                column: "RefreshToken");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_SessionToken",
                table: "UserSessions",
                column: "SessionToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDocuments_AssetRegisters_AssetRegisterId",
                table: "AssetDocuments",
                column: "AssetRegisterId",
                principalTable: "AssetRegisters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_AssetInsurances_Assets_AssetId",
                table: "AssetInsurances",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetMaintenances_Assets_AssetId",
                table: "AssetMaintenances",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetMaintenances_Transactions_TransactionId",
                table: "AssetMaintenances",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRegisters_Assets_AssetId",
                table: "AssetRegisters",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Transactions_PurchaseTransactionId",
                table: "Assets",
                column: "PurchaseTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_UserId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Users_UserId",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UserId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Assets_AssetId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.DropTable(
                name: "AssetDocuments");

            migrationBuilder.DropTable(
                name: "AssetInsurances");

            migrationBuilder.DropTable(
                name: "AssetMaintenances");

            migrationBuilder.DropTable(
                name: "AuditEntries");

            migrationBuilder.DropTable(
                name: "BudgetAlerts");

            migrationBuilder.DropTable(
                name: "BudgetCategories");

            migrationBuilder.DropTable(
                name: "BudgetDebt");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "DebtPayments");

            migrationBuilder.DropTable(
                name: "Incomes");

            migrationBuilder.DropTable(
                name: "InsuranceBeneficiaries");

            migrationBuilder.DropTable(
                name: "InsuranceDocuments");

            migrationBuilder.DropTable(
                name: "InsuranceNotifications");

            migrationBuilder.DropTable(
                name: "InsurancePremiumPayments");

            migrationBuilder.DropTable(
                name: "MaintenanceRecords");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "NotificationHistory");

            migrationBuilder.DropTable(
                name: "ObligationDocuments");

            migrationBuilder.DropTable(
                name: "ObligationNotifications");

            migrationBuilder.DropTable(
                name: "ReconciliationItems");

            migrationBuilder.DropTable(
                name: "ReportParameters");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "SplitTransactions");

            migrationBuilder.DropTable(
                name: "TransactionAttachments");

            migrationBuilder.DropTable(
                name: "TransactionNotifications");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "IncomeCategories");

            migrationBuilder.DropTable(
                name: "InsuranceClaims");

            migrationBuilder.DropTable(
                name: "AssetRegisters");

            migrationBuilder.DropTable(
                name: "NotificationRules");

            migrationBuilder.DropTable(
                name: "ObligationBenefits");

            migrationBuilder.DropTable(
                name: "ObligationPayments");

            migrationBuilder.DropTable(
                name: "ReconciliationSessions");

            migrationBuilder.DropTable(
                name: "ReportTemplates");

            migrationBuilder.DropTable(
                name: "Insurances");

            migrationBuilder.DropTable(
                name: "NotificationSettings");

            migrationBuilder.DropTable(
                name: "Obligations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "AssetCategories");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "Investments");

            migrationBuilder.DropTable(
                name: "ScheduledTransactions");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Debts");

            migrationBuilder.DropTable(
                name: "Creditors");
        }
    }
}
