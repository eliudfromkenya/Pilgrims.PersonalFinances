using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Data;

public class PersonalFinanceContext : DbContext
{
    public PersonalFinanceContext(DbContextOptions<PersonalFinanceContext> options) : base(options)
    {
    }

    // Parameterless constructor for EF migrations
    public PersonalFinanceContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Use a default path for testing - in production this will be overridden
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PersonalFinance.db");
            var connectionString = $"Data Source={dbPath};";
            optionsBuilder.UseSqlite(connectionString);
        }
        
        // Suppress the pending model changes warning
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    // Core Financial Entities
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<SplitTransaction> SplitTransactions { get; set; }
    public DbSet<TransactionAttachment> TransactionAttachments { get; set; }
    public DbSet<Category> Categories { get; set; }

    // Scheduled Transactions
    public DbSet<ScheduledTransaction> ScheduledTransactions { get; set; }
    public DbSet<TransactionNotification> TransactionNotifications { get; set; }

    // Budget Management
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<BudgetCategory> BudgetCategories { get; set; }
    public DbSet<BudgetAlert> BudgetAlerts { get; set; }

    // Income Management
    public DbSet<Income> Incomes { get; set; }
    public DbSet<IncomeCategory> IncomeCategories { get; set; }

    // Debt & Creditor Management
    public DbSet<Debt> Debts { get; set; }
    public DbSet<Creditor> Creditors { get; set; }
    public DbSet<DebtPayment> DebtPayments { get; set; }

    // Asset Management
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetCategory> AssetCategories { get; set; }
    public DbSet<AssetDocument> AssetDocuments { get; set; }
    public DbSet<AssetInsurance> AssetInsurances { get; set; }
    public DbSet<AssetMaintenance> AssetMaintenances { get; set; }
    public DbSet<AssetRegister> AssetRegisters { get; set; }
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

    // Investment Management
    public DbSet<Investment> Investments { get; set; }

    // Reconciliation
    public DbSet<ReconciliationSession> ReconciliationSessions { get; set; }
    public DbSet<ReconciliationItem> ReconciliationItems { get; set; }

    // Notification System
    public DbSet<NotificationRule> NotificationRules { get; set; }
    public DbSet<NotificationHistory> NotificationHistory { get; set; }
    public DbSet<NotificationSettings> NotificationSettings { get; set; }

    // Insurance & Obligations
    public DbSet<Insurance> Insurances { get; set; }
    public DbSet<InsurancePremiumPayment> InsurancePremiumPayments { get; set; }
    public DbSet<InsuranceClaim> InsuranceClaims { get; set; }
    public DbSet<InsuranceBeneficiary> InsuranceBeneficiaries { get; set; }
    public DbSet<InsuranceDocument> InsuranceDocuments { get; set; }
    public DbSet<InsuranceNotification> InsuranceNotifications { get; set; }
    public DbSet<Obligation> Obligations { get; set; }
    public DbSet<ObligationPayment> ObligationPayments { get; set; }
    public DbSet<ObligationBenefit> ObligationBenefits { get; set; }
    public DbSet<ObligationDocument> ObligationDocuments { get; set; }
    public DbSet<ObligationNotification> ObligationNotifications { get; set; }

    // Reporting System
    public DbSet<Report> Reports { get; set; }
    public DbSet<ReportTemplate> ReportTemplates { get; set; }
    public DbSet<ReportParameter> ReportParameters { get; set; }

    // Goal Management
    public DbSet<Goal> Goals { get; set; }

    // User Management
    public DbSet<User> Users { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure all string properties to use TEXT type for SQLite compatibility
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string))
                {
                    property.SetColumnType("TEXT");
                }
            }
        }

        base.OnModelCreating(modelBuilder);

        // Configure Account entity
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.CurrentBalance).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.InitialBalance).HasPrecision(18, 2);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.BankName).HasMaxLength(100);

            entity.HasIndex(e => e.AccountNumber).IsUnique().HasFilter("[AccountNumber] IS NOT NULL");
            entity.HasIndex(e => e.AccountType);
        });

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Payee).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
            entity.Property(e => e.Tags).HasMaxLength(500);

            entity.HasOne(e => e.Account)
                  .WithMany(a => a.Transactions)
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Transactions)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.AccountId);
            entity.HasIndex(e => e.CategoryId);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ColorCode).IsRequired().HasMaxLength(7);
            entity.Property(e => e.IconName).HasMaxLength(50);

            entity.HasOne(e => e.ParentCategory)
                  .WithMany(c => c.SubCategories)
                  .HasForeignKey(e => e.ParentCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.ParentCategoryId);
        });

        // Configure Budget entity
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.LimitAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.SpentAmount).HasPrecision(18, 2);
            entity.Property(e => e.RolloverAmount).HasPrecision(18, 2);
            entity.Property(e => e.Tag).HasMaxLength(50);
            entity.Property(e => e.AlertLevels).HasMaxLength(20);

            entity.HasOne(e => e.Category)
                  .WithMany()
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Account)
                  .WithMany()
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.EndDate);
            entity.HasIndex(e => e.IsActive);
        });

        // Configure BudgetCategory entity
        modelBuilder.Entity<BudgetCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AllocatedAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.SpentAmount).HasPrecision(18, 2);
            entity.Property(e => e.TagName).HasMaxLength(100);

            entity.HasOne(e => e.Budget)
                  .WithMany()
                  .HasForeignKey(e => e.BudgetId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                  .WithMany()
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Account)
                  .WithMany()
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.BudgetId, e.CategoryId }).IsUnique();
        });

        // Configure BudgetAlert entity
        modelBuilder.Entity<BudgetAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BudgetId).IsRequired();
            entity.Property(e => e.BudgetAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.SpentAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.UsedPercentage).HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.Message).IsRequired();

            entity.HasIndex(e => e.BudgetId);
        });

        // Configure ScheduledTransaction entity
        modelBuilder.Entity<ScheduledTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Payee).HasMaxLength(200);

            entity.HasOne(e => e.Account)
                  .WithMany()
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                  .WithMany()
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.NextDueDate);
            entity.HasIndex(e => e.IsActive);
        });

        // Configure TransactionNotification entity
        modelBuilder.Entity<TransactionNotification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.ScheduledTransactionId).IsRequired();

            entity.HasIndex(e => e.ScheduledDate);
            entity.HasIndex(e => e.IsRead);
        });

        // Configure Income entity
        modelBuilder.Entity<Income>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.NetAmount).HasPrecision(18, 2);
            entity.Property(e => e.TaxRate).HasPrecision(5, 2);
            entity.Property(e => e.Source).HasMaxLength(100);
            entity.Property(e => e.PaymentMethod).HasMaxLength(100);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(100);
            entity.Property(e => e.ReceivedDate).IsRequired();

            entity.HasOne(e => e.IncomeCategory)
                  .WithMany(c => c.Incomes)
                  .HasForeignKey(e => e.IncomeCategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.ReceivedDate);
            entity.HasIndex(e => e.IncomeType);
            entity.HasIndex(e => e.IsRegular);
        });

        // Configure IncomeCategory entity
        modelBuilder.Entity<IncomeCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Color).HasMaxLength(7);
            entity.Property(e => e.Icon).HasMaxLength(50);

            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Creditor entity
        modelBuilder.Entity<Creditor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Website).HasMaxLength(200);
            entity.Property(e => e.CustomerServicePhone).HasMaxLength(20);
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Debt entity
        modelBuilder.Entity<Debt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PrincipalAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.CurrentBalance).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.InterestRate).HasPrecision(5, 4);
            entity.Property(e => e.MinimumPayment).HasPrecision(18, 2);
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.Creditor)
                  .WithMany(c => c.Debts)
                  .HasForeignKey(e => e.CreditorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.CreditorId, e.AccountNumber })
                  .IsUnique()
                  .HasFilter("[AccountNumber] IS NOT NULL");
        });

        // Configure DebtPayment entity
        modelBuilder.Entity<DebtPayment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.PaymentDate).IsRequired();
            entity.Property(e => e.PaymentMethod).HasMaxLength(100);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Debt)
                  .WithMany(d => d.Payments)
                  .HasForeignKey(e => e.DebtId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PaymentDate);
            entity.HasIndex(e => new { e.DebtId, e.PaymentDate });
        });

        // Configure Asset entity
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.PurchasePrice).HasPrecision(18, 2);
            entity.Property(e => e.CurrentValue).HasPrecision(18, 2);
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Condition).HasMaxLength(100);

            entity.HasOne(e => e.AssetCategory)
                  .WithMany(c => c.Assets)
                  .HasForeignKey(e => e.AssetCategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.SerialNumber).IsUnique().HasFilter("[SerialNumber] IS NOT NULL");
            entity.HasIndex(e => e.PurchaseDate);
        });

        // Configure AssetCategory entity
        modelBuilder.Entity<AssetCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Color).HasMaxLength(7);
            entity.Property(e => e.Icon).HasMaxLength(50);

            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Investment entity
        modelBuilder.Entity<Investment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Symbol).HasMaxLength(20);
            entity.Property(e => e.Quantity).HasPrecision(18, 6);
            entity.Property(e => e.PurchasePrice).HasPrecision(18, 4);
            entity.Property(e => e.CurrentPrice).HasPrecision(18, 4);
            // Remove TotalValue mapping since it's a computed property with [NotMapped]

            entity.HasOne(e => e.Account)
                  .WithMany()
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Symbol);
            entity.HasIndex(e => e.PurchaseDate);
        });

        // Configure ReconciliationSession entity
        modelBuilder.Entity<ReconciliationSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SessionName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ReconciliationDate).IsRequired();
            entity.Property(e => e.StatementStartDate).IsRequired();
            entity.Property(e => e.StatementEndDate).IsRequired();
            entity.Property(e => e.StatementStartingBalance).HasPrecision(18, 2);
            entity.Property(e => e.StatementEndingBalance).HasPrecision(18, 2);
            entity.Property(e => e.BookStartingBalance).HasPrecision(18, 2);
            entity.Property(e => e.BookEndingBalance).HasPrecision(18, 2);
            entity.Property(e => e.Difference).HasPrecision(18, 2);

            entity.HasOne(e => e.Account)
                  .WithMany()
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.ReconciliationDate);
            entity.HasIndex(e => e.Status);
        });

        // Configure ReconciliationItem entity
        modelBuilder.Entity<ReconciliationItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.TransactionDate).IsRequired();

            entity.HasOne(e => e.ReconciliationSession)
                  .WithMany(s => s.ReconciliationItems)
                  .HasForeignKey(e => e.ReconciliationSessionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Transaction)
                  .WithMany()
                  .HasForeignKey(e => e.TransactionId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.TransactionDate);
            entity.HasIndex(e => e.Status);
        });

        // Configure Report entity
        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ReportType).IsRequired();
            entity.Property(e => e.Status).IsRequired();

            entity.HasOne(e => e.ReportTemplate)
                  .WithMany()
                  .HasForeignKey(e => e.ReportTemplateId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.ReportType);
            entity.HasIndex(e => e.Status);
        });

        // Configure ReportTemplate entity
        modelBuilder.Entity<ReportTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ReportType).IsRequired();
            entity.Property(e => e.DefaultChartType).IsRequired();

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.ReportType);
            entity.HasIndex(e => e.IsSystemTemplate);
        });

        // Configure ReportParameter entity
        modelBuilder.Entity<ReportParameter>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Label).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ParameterType).IsRequired();

            entity.HasOne(e => e.ReportTemplate)
                  .WithMany()
                  .HasForeignKey(e => e.ReportTemplateId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.ParameterType);
        });

        // Configure NotificationRule entity
        modelBuilder.Entity<NotificationRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.NotificationType).IsRequired();
            entity.Property(e => e.Priority).IsRequired();
            entity.Property(e => e.Frequency).IsRequired();

            entity.HasOne(e => e.NotificationSettings)
                  .WithMany()
                  .HasForeignKey(e => e.NotificationSettingsId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.NotificationType);
        });

        // Configure NotificationHistory entity
        modelBuilder.Entity<NotificationHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.ScheduledAt).IsRequired();
            entity.Property(e => e.NotificationType).IsRequired();
            entity.Property(e => e.Channel).IsRequired();
            entity.Property(e => e.Priority).IsRequired();
            entity.Property(e => e.Status).IsRequired();

            entity.HasOne(e => e.NotificationRule)
                  .WithMany()
                  .HasForeignKey(e => e.NotificationRuleId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.ScheduledAt);
            entity.HasIndex(e => e.SentAt);
            entity.HasIndex(e => e.ReadAt);
            entity.HasIndex(e => e.Status);
        });

        // Configure NotificationSettings entity
        modelBuilder.Entity<NotificationSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NotificationType).IsRequired();
            entity.Property(e => e.IsEnabled).IsRequired();
            entity.Property(e => e.PreferredChannels).IsRequired();
            entity.Property(e => e.PreferredTime).IsRequired();

            entity.HasIndex(e => e.NotificationType);
        });

        // Configure AssetDocument entity
        modelBuilder.Entity<AssetDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileExtension).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(e => e.AssetRegister)
                  .WithMany(a => a.Documents)
                  .HasForeignKey(e => e.AssetRegisterId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.AssetRegisterId);
        });

        // Configure TransactionAttachment entity
        modelBuilder.Entity<TransactionAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.OriginalFileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(e => e.Transaction)
                  .WithMany(t => t.Attachments)
                  .HasForeignKey(e => e.TransactionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.TransactionId);
        });

        // Configure SplitTransaction entity
        modelBuilder.Entity<SplitTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Memo).HasMaxLength(500);
            entity.Property(e => e.Tags).HasMaxLength(1000);

            entity.HasOne(e => e.Transaction)
                  .WithMany()
                  .HasForeignKey(e => e.TransactionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                  .WithMany()
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.TransactionId);
        });

        // Configure AssetInsurance entity
        modelBuilder.Entity<AssetInsurance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PolicyNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.InsuranceProvider).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AgentName).HasMaxLength(100);
            entity.Property(e => e.AgentContact).HasMaxLength(100);
            entity.Property(e => e.CoverageAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.Deductible).HasPrecision(18, 2);
            entity.Property(e => e.CoverageType).HasMaxLength(100);
            entity.Property(e => e.CoverageDetails).HasMaxLength(1000);
            entity.Property(e => e.AnnualPremium).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.MonthlyPremium).HasPrecision(18, 2);
            entity.Property(e => e.PaymentFrequency).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.PrimaryBeneficiary).HasMaxLength(200);
            entity.Property(e => e.SecondaryBeneficiary).HasMaxLength(200);
            entity.Property(e => e.TotalClaimsAmount).HasPrecision(18, 2);
            entity.Property(e => e.PolicyDocumentPath).HasMaxLength(1000);
            entity.Property(e => e.CertificatePath).HasMaxLength(1000);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.Asset)
                  .WithMany(a => a.InsurancePolicies)
                  .HasForeignKey(e => e.AssetId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ScheduledTransaction)
                  .WithMany()
                  .HasForeignKey(e => e.ScheduledTransactionId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.PolicyNumber);
            entity.HasIndex(e => e.PolicyEndDate);
            entity.HasIndex(e => e.IsActive);
        });

        // Configure AssetMaintenance entity
        modelBuilder.Entity<AssetMaintenance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ServiceType).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Cost).HasPrecision(18, 2);
            entity.Property(e => e.ServiceProvider).HasMaxLength(200);
            entity.Property(e => e.ServiceProviderContact).HasMaxLength(100);
            entity.Property(e => e.ServiceCategory).HasMaxLength(50);
            entity.Property(e => e.Priority).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.WarrantyDetails).HasMaxLength(500);
            entity.Property(e => e.ReceiptPath).HasMaxLength(1000);
            entity.Property(e => e.ServiceReportPath).HasMaxLength(1000);
            entity.Property(e => e.PartsCost).HasPrecision(18, 2);
            entity.Property(e => e.LaborCost).HasPrecision(18, 2);

            entity.HasOne(e => e.Asset)
                  .WithMany(a => a.MaintenanceRecords)
                  .HasForeignKey(e => e.AssetId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Transaction)
                  .WithMany()
                  .HasForeignKey(e => e.TransactionId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.ServiceDate);
            entity.HasIndex(e => e.NextServiceDue);
            entity.HasIndex(e => e.Status);
        });

        // Configure AssetRegister entity
        modelBuilder.Entity<AssetRegister>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PurchaseReceiptPath).HasMaxLength(1000);
            entity.Property(e => e.PurchasePrice).HasPrecision(18, 2);
            entity.Property(e => e.Vendor).HasMaxLength(200);
            entity.Property(e => e.PurchaseOrderNumber).HasMaxLength(100);
            entity.Property(e => e.WarrantyDocumentPath).HasMaxLength(1000);
            entity.Property(e => e.WarrantyProvider).HasMaxLength(200);
            entity.Property(e => e.WarrantyTerms).HasMaxLength(1000);
            entity.Property(e => e.InsurancePolicyPath).HasMaxLength(1000);
            entity.Property(e => e.InsurancePolicyNumber).HasMaxLength(100);
            entity.Property(e => e.InsuranceProvider).HasMaxLength(200);
            entity.Property(e => e.InsuranceCoverage).HasPrecision(18, 2);
            entity.Property(e => e.InsurancePremium).HasPrecision(18, 2);
            entity.Property(e => e.MaintenanceSchedule).HasMaxLength(500);
            entity.Property(e => e.MaintenanceNotes).HasMaxLength(1000);
            entity.Property(e => e.ManualPath).HasMaxLength(1000);
            entity.Property(e => e.CertificatesPath).HasMaxLength(1000);
            entity.Property(e => e.AdditionalDocumentsPath).HasMaxLength(1000);
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.ModelNumber).HasMaxLength(100);
            entity.Property(e => e.Manufacturer).HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.ResponsiblePerson).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(e => e.Asset)
                  .WithOne(a => a.AssetRegister)
                  .HasForeignKey<AssetRegister>(e => e.AssetId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.SerialNumber).IsUnique().HasFilter("[SerialNumber] IS NOT NULL");
            entity.HasIndex(e => e.PurchaseDate);
        });

        // Configure MaintenanceRecord entity
        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MaintenanceType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Cost).HasPrecision(18, 2);
            entity.Property(e => e.ServiceProvider).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.AssetRegister)
                  .WithMany(a => a.MaintenanceRecords)
                  .HasForeignKey(e => e.AssetRegisterId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.MaintenanceDate);
            entity.HasIndex(e => e.NextMaintenanceDate);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.PasswordSalt).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.TimeZone).HasMaxLength(50);
            entity.Property(e => e.EmailVerificationToken).HasMaxLength(500);
            entity.Property(e => e.PasswordResetToken).HasMaxLength(500);
            entity.Property(e => e.ProfilePicturePath).HasMaxLength(500);
            entity.Property(e => e.Preferences).HasColumnType("TEXT");

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.EmailVerificationToken);
            entity.HasIndex(e => e.PasswordResetToken);

            // Configure relationships
            entity.HasMany(e => e.Accounts)
                  .WithOne()
                  .HasForeignKey("UserId")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Budgets)
                  .WithOne()
                  .HasForeignKey("UserId")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Categories)
                  .WithOne()
                  .HasForeignKey("UserId")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Transactions)
                  .WithOne()
                  .HasForeignKey("UserId")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure UserSession entity
        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SessionToken).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.RefreshToken).HasMaxLength(1000);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.DeviceInfo).HasMaxLength(200);
            entity.Property(e => e.RevocationReason).HasMaxLength(200);

            entity.HasIndex(e => e.SessionToken).IsUnique();
            entity.HasIndex(e => e.RefreshToken);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ExpiresAt);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Insurance entity
        modelBuilder.Entity<Insurance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PolicyNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.InsuranceCompany).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PolicyType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CoverageAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.PremiumAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.DeductibleAmount).HasPrecision(18, 2);
            entity.Property(e => e.BeneficiaryName).HasMaxLength(200);
            entity.Property(e => e.BeneficiaryRelationship).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasIndex(e => e.PolicyNumber).IsUnique();
            entity.HasIndex(e => e.PolicyEndDate);
            entity.HasIndex(e => e.Status);
        });

        // Configure InsuranceClaim entity
        modelBuilder.Entity<InsuranceClaim>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClaimNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ClaimAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.ApprovedAmount).HasPrecision(18, 2);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.Insurance)
                  .WithMany(i => i.Claims)
                  .HasForeignKey(e => e.InsuranceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ClaimNumber).IsUnique();
            entity.HasIndex(e => e.ClaimDate);
            entity.HasIndex(e => e.Status);
        });

        // Configure InsuranceBeneficiary entity
        modelBuilder.Entity<InsuranceBeneficiary>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Relationship).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Percentage).HasPrecision(5, 2);
            entity.Property(e => e.IdentificationNumber).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Insurance)
                  .WithMany(i => i.Beneficiaries)
                  .HasForeignKey(e => e.InsuranceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.FullName);
            entity.HasIndex(e => e.IsActive);
        });

        // Configure InsuranceDocument entity
        modelBuilder.Entity<InsuranceDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DocumentType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(e => e.Insurance)
                  .WithMany(i => i.Documents)
                  .HasForeignKey(e => e.InsuranceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Claim)
                  .WithMany(c => c.Documents)
                  .HasForeignKey(e => e.ClaimId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.DocumentType);
            entity.HasIndex(e => e.UploadDate);
        });

        // Configure InsuranceNotification entity
        modelBuilder.Entity<InsuranceNotification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.AdditionalData).HasMaxLength(500);

            entity.HasOne(e => e.Insurance)
                  .WithMany()
                  .HasForeignKey(e => e.InsuranceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.InsuranceClaim)
                  .WithMany()
                  .HasForeignKey(e => e.InsuranceClaimId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.ScheduledDate);
            entity.HasIndex(e => e.IsSent);
        });

        // Configure Obligation entity
        modelBuilder.Entity<Obligation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ContributionAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.OrganizationName).HasMaxLength(200);
            entity.Property(e => e.ContactPerson).HasMaxLength(100);
            entity.Property(e => e.MembershipNumber).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Status);
        });

        // Configure ObligationPayment entity
        modelBuilder.Entity<ObligationPayment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.PaymentMethod).HasMaxLength(100);
            entity.Property(e => e.TransactionReference).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Obligation)
                  .WithMany(o => o.Payments)
                  .HasForeignKey(e => e.ObligationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PaymentDate);
            entity.HasIndex(e => new { e.ObligationId, e.PaymentDate });
        });

        // Configure ObligationBenefit entity
        modelBuilder.Entity<ObligationBenefit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BenefitType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Obligation)
                  .WithMany(o => o.Benefits)
                  .HasForeignKey(e => e.ObligationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ReceivedDate);
            entity.HasIndex(e => e.Status);
        });

        // Configure ObligationDocument entity
        modelBuilder.Entity<ObligationDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.DocumentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(e => e.Obligation)
                  .WithMany(o => o.Documents)
                  .HasForeignKey(e => e.ObligationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ObligationId);
        });

        // Configure ObligationNotification entity
        modelBuilder.Entity<ObligationNotification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.AdditionalData).HasMaxLength(500);

            entity.HasOne(e => e.Obligation)
                  .WithMany()
                  .HasForeignKey(e => e.ObligationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ObligationPayment)
                  .WithMany()
                  .HasForeignKey(e => e.ObligationPaymentId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.ObligationBenefit)
                  .WithMany()
                  .HasForeignKey(e => e.ObligationBenefitId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.ScheduledDate);
            entity.HasIndex(e => e.IsSent);
        });

        // Configure Goal entity
        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.TargetAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.CurrentAmount).HasPrecision(18, 2);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Icon).HasMaxLength(20);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasIndex(e => e.GoalType);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.TargetDate);
        });

        // Seed data for categories if needed
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = "1", Name = "Debt Payment", ColorCode = "#FF6B6B", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = "2", Name = "Interest", ColorCode = "#FF4757", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = "3", Name = "Credit Card", ColorCode = "#FF3838", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<ITimestampedEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    ((BaseEntity)entry.Entity).TouchUpdatedAt();
                    break;
                case EntityState.Modified:
                    ((BaseEntity)entry.Entity).TouchUpdatedAt();
                    break;
            }
        }
    }
}

public interface ITimestampedEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}