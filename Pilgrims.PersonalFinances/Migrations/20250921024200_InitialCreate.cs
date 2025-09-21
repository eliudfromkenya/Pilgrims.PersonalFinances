using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pilgrims.PersonalFinances.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creditors", x => x.Id);
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
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
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
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IncomeCategoryId = table.Column<string>(type: "TEXT", nullable: true),
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
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AccountType = table.Column<int>(type: "INTEGER", nullable: false),
                    InitialBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
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
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
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
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
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
                    AlertLevels = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AlertsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastAlertLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
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
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
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
                    AssetCategoryId = table.Column<string>(type: "TEXT", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DepreciationMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DepreciationRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    UsefulLifeYears = table.Column<int>(type: "INTEGER", nullable: true),
                    SalvageValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
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
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AssetRegisterId = table.Column<string>(type: "TEXT", nullable: true),
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
                    AssetId = table.Column<string>(type: "TEXT", nullable: true),
                    BudgetId = table.Column<string>(type: "TEXT", nullable: true),
                    DebtId = table.Column<string>(type: "TEXT", nullable: true),
                    InvestmentId = table.Column<string>(type: "TEXT", nullable: true),
                    ScheduledTransactionId = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NotificationRuleId1 = table.Column<string>(type: "TEXT", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationHistory_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationHistory_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationHistory_Debts_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debts",
                        principalColumn: "Id");
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationHistory_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
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
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
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
                    { "1", "#FF6B6B", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, true, "Debt Payment", null, new DateTime(2025, 9, 21, 2, 41, 57, 282, DateTimeKind.Utc).AddTicks(339), null },
                    { "2", "#FF4757", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, true, "Interest", null, new DateTime(2025, 9, 21, 2, 41, 57, 283, DateTimeKind.Utc).AddTicks(720), null },
                    { "3", "#FF3838", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, true, "Credit Card", null, new DateTime(2025, 9, 21, 2, 41, 57, 283, DateTimeKind.Utc).AddTicks(732), null }
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
                name: "IX_AssetCategories_Name",
                table: "AssetCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetDocuments_AssetRegisterId",
                table: "AssetDocuments",
                column: "AssetRegisterId");

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
                principalColumn: "Id");
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
                name: "AssetDocuments");

            migrationBuilder.DropTable(
                name: "AssetInsurances");

            migrationBuilder.DropTable(
                name: "AssetMaintenances");

            migrationBuilder.DropTable(
                name: "BudgetAlerts");

            migrationBuilder.DropTable(
                name: "BudgetCategories");

            migrationBuilder.DropTable(
                name: "BudgetDebt");

            migrationBuilder.DropTable(
                name: "DebtPayments");

            migrationBuilder.DropTable(
                name: "Incomes");

            migrationBuilder.DropTable(
                name: "MaintenanceRecords");

            migrationBuilder.DropTable(
                name: "NotificationHistory");

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
                name: "IncomeCategories");

            migrationBuilder.DropTable(
                name: "AssetRegisters");

            migrationBuilder.DropTable(
                name: "NotificationRules");

            migrationBuilder.DropTable(
                name: "ReconciliationSessions");

            migrationBuilder.DropTable(
                name: "ReportTemplates");

            migrationBuilder.DropTable(
                name: "NotificationSettings");

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
                name: "Debts");

            migrationBuilder.DropTable(
                name: "Investments");

            migrationBuilder.DropTable(
                name: "ScheduledTransactions");

            migrationBuilder.DropTable(
                name: "Creditors");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
