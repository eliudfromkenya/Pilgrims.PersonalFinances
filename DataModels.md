# Data Models and Database Schema

**Project:** Personal Finance Manager (Offline, .NET MAUI)  
**Version:** 1.0  
**Date:** January 2025  
**Database:** SQLite with Entity Framework Core  

## Table of Contents
1. [Database Design Overview](#1-database-design-overview)
2. [Entity Relationship Diagram](#2-entity-relationship-diagram)
3. [Core Entities](#3-core-entities)
4. [Supporting Entities](#4-supporting-entities)
5. [Database Indexes](#5-database-indexes)
6. [Entity Framework Models](#6-entity-framework-models)
7. [Database Migrations](#7-database-migrations)
8. [Data Validation Rules](#8-data-validation-rules)

---

## 1. Database Design Overview

### 1.1 Design Principles
- **Normalization:** Database designed to 3rd Normal Form to minimize redundancy
- **Referential Integrity:** Foreign key constraints ensure data consistency
- **Performance:** Strategic indexing for optimal query performance
- **Scalability:** Schema supports growth to 100,000+ transactions
- **Audit Trail:** Created/modified timestamps on all entities
- **Soft Deletes:** Logical deletion to preserve data integrity

### 1.2 Naming Conventions
- **Tables:** PascalCase (e.g., `Accounts`, `Transactions`)
- **Columns:** PascalCase (e.g., `AccountId`, `TransactionDate`)
- **Indexes:** `IX_TableName_ColumnName` format
- **Foreign Keys:** `FK_ChildTable_ParentTable_Column` format
- **Primary Keys:** Always named `Id` with AUTOINCREMENT

### 1.3 Data Types
- **Monetary Values:** `DECIMAL(18,2)` for precision
- **Dates:** `DATE` for date-only, `DATETIME` for timestamps
- **Text:** `NVARCHAR` with appropriate length limits
- **Booleans:** `BOOLEAN` (SQLite INTEGER 0/1)
- **Enums:** `INTEGER` with documented value mappings

---

## 2. Entity Relationship Diagram

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Accounts  │    │ Categories  │    │   Budgets   │
│             │    │             │    │             │
│ Id (PK)     │    │ Id (PK)     │    │ Id (PK)     │
│ Name        │    │ Name        │    │ CategoryId  │──┐
│ Type        │    │ ParentId    │──┐ │ Amount      │  │
│ Currency    │    │ Type        │  │ │ Period      │  │
│ Balance     │    │ Color       │  │ └─────────────┘  │
└─────────────┘    └─────────────┘  │                  │
       │                    │       │                  │
       │                    └───────┼──────────────────┘
       │                            │
       │           ┌─────────────────┼─────────────┐
       │           │                 │             │
       ▼           ▼                 ▼             │
┌─────────────────────────────────────────────────┐│
│                Transactions                     ││
│                                                 ││
│ Id (PK)                                         ││
│ AccountId (FK) ──────────────────────────────────┘
│ CategoryId (FK) ────────────────────────────────┐
│ Amount                                          │
│ Date                                            │
│ Type                                            │
│ Description                                     │
│ Status                                          │
│ ParentTransactionId (FK) ──┐                    │
└─────────────────────────────┼───────────────────┘
                              │
              ┌───────────────┘
              │
              ▼
┌─────────────────────────────┐    ┌─────────────┐
│    TransactionTags          │    │    Debts    │
│                             │    │             │
│ Id (PK)                     │    │ Id (PK)     │
│ TransactionId (FK)          │    │ Name        │
│ TagName                     │    │ Amount      │
└─────────────────────────────┘    │ Balance     │
                                   │ DueDate     │
┌─────────────────────────────┐    │ Rate        │
│   ScheduledTransactions     │    └─────────────┘
│                             │
│ Id (PK)                     │    ┌─────────────┐
│ AccountId (FK)              │    │   Assets    │
│ CategoryId (FK)             │    │             │
│ Amount                      │    │ Id (PK)     │
│ RecurrenceType              │    │ Name        │
│ NextDueDate                 │    │ Category    │
│ AutoPost                    │    │ Value       │
└─────────────────────────────┘    │ PurchaseDate│
                                   └─────────────┘
┌─────────────────────────────┐
│       Attachments           │    ┌─────────────┐
│                             │    │Notifications│
│ Id (PK)                     │    │             │
│ ParentType                  │    │ Id (PK)     │
│ ParentId                    │    │ Title       │
│ FileName                    │    │ Message     │
│ FilePath                    │    │ DueDate     │
│ FileSize                    │    │ IsRead      │
└─────────────────────────────┘    └─────────────┘
```

---

## 3. Core Entities

### 3.1 Accounts Table

```sql
CREATE TABLE Accounts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name NVARCHAR(100) NOT NULL,
    AccountType INTEGER NOT NULL,
    Currency NVARCHAR(3) NOT NULL DEFAULT 'USD',
    OpeningBalance DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    CurrentBalance DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    ColorCode NVARCHAR(7) NULL,
    IsActive BOOLEAN NOT NULL DEFAULT 1,
    SortOrder INTEGER NOT NULL DEFAULT 0,
    Notes NVARCHAR(500) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT CK_Accounts_Currency CHECK (LENGTH(Currency) = 3),
    CONSTRAINT CK_Accounts_ColorCode CHECK (ColorCode IS NULL OR ColorCode LIKE '#%'),
    CONSTRAINT CK_Accounts_Name CHECK (LENGTH(TRIM(Name)) > 0)
);
```

**Account Types Enum:**
```csharp
public enum AccountType
{
    Bank = 1,           // Checking, Savings accounts
    Cash = 2,           // Physical cash
    CreditCard = 3,     // Credit card accounts
    Investment = 4,     // Investment accounts
    Loan = 5,           // Loan accounts
    Asset = 6           // Asset accounts (house, car)
}
```

### 3.2 Transactions Table

```sql
CREATE TABLE Transactions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    AccountId INTEGER NOT NULL,
    TransactionType INTEGER NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    TransactionDate DATE NOT NULL,
    CategoryId INTEGER NULL,
    PayeePayerName NVARCHAR(200) NULL,
    Description NVARCHAR(500) NULL,
    Status INTEGER NOT NULL DEFAULT 1,
    TransferAccountId INTEGER NULL,
    ParentTransactionId INTEGER NULL,
    CheckNumber NVARCHAR(20) NULL,
    ReferenceNumber NVARCHAR(50) NULL,
    IsReconciled BOOLEAN NOT NULL DEFAULT 0,
    ReconciliationDate DATE NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE RESTRICT,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE SET NULL,
    FOREIGN KEY (TransferAccountId) REFERENCES Accounts(Id) ON DELETE SET NULL,
    FOREIGN KEY (ParentTransactionId) REFERENCES Transactions(Id) ON DELETE CASCADE,
    
    CONSTRAINT CK_Transactions_Amount CHECK (Amount != 0),
    CONSTRAINT CK_Transactions_TransferAccount CHECK (
        (TransactionType = 3 AND TransferAccountId IS NOT NULL) OR 
        (TransactionType != 3 AND TransferAccountId IS NULL)
    )
);
```

**Transaction Types Enum:**
```csharp
public enum TransactionType
{
    Income = 1,         // Money coming in
    Expense = 2,        // Money going out
    Transfer = 3        // Transfer between accounts
}
```

**Transaction Status Enum:**
```csharp
public enum TransactionStatus
{
    Pending = 1,        // Not yet cleared
    Cleared = 2,        // Cleared by bank
    Reconciled = 3,     // Reconciled with statement
    Void = 4           // Voided transaction
}
```

### 3.3 Categories Table

```sql
CREATE TABLE Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name NVARCHAR(100) NOT NULL,
    ParentCategoryId INTEGER NULL,
    CategoryType INTEGER NOT NULL,
    ColorCode NVARCHAR(7) NULL,
    IconName NVARCHAR(50) NULL,
    IsActive BOOLEAN NOT NULL DEFAULT 1,
    IsSystemCategory BOOLEAN NOT NULL DEFAULT 0,
    SortOrder INTEGER NOT NULL DEFAULT 0,
    Description NVARCHAR(200) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (ParentCategoryId) REFERENCES Categories(Id) ON DELETE SET NULL,
    
    CONSTRAINT CK_Categories_Name CHECK (LENGTH(TRIM(Name)) > 0),
    CONSTRAINT CK_Categories_ColorCode CHECK (ColorCode IS NULL OR ColorCode LIKE '#%'),
    CONSTRAINT CK_Categories_NoSelfReference CHECK (Id != ParentCategoryId)
);
```

**Category Types Enum:**
```csharp
public enum CategoryType
{
    Income = 1,         // Income categories
    Expense = 2,        // Expense categories
    Transfer = 3        // Transfer categories
}
```

### 3.4 Budgets Table

```sql
CREATE TABLE Budgets (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name NVARCHAR(100) NOT NULL,
    CategoryId INTEGER NULL,
    AccountId INTEGER NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PeriodType INTEGER NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    AllowRollover BOOLEAN NOT NULL DEFAULT 0,
    RolloverAmount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    AlertThreshold DECIMAL(5,2) NOT NULL DEFAULT 80.00,
    IsActive BOOLEAN NOT NULL DEFAULT 1,
    Notes NVARCHAR(500) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE,
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE,
    
    CONSTRAINT CK_Budgets_Amount CHECK (Amount > 0),
    CONSTRAINT CK_Budgets_AlertThreshold CHECK (AlertThreshold BETWEEN 0 AND 100),
    CONSTRAINT CK_Budgets_CategoryOrAccount CHECK (
        (CategoryId IS NOT NULL AND AccountId IS NULL) OR 
        (CategoryId IS NULL AND AccountId IS NOT NULL)
    )
);
```

**Budget Period Types Enum:**
```csharp
public enum BudgetPeriodType
{
    Weekly = 1,         // Weekly budget
    BiWeekly = 2,       // Every 2 weeks
    Monthly = 3,        // Monthly budget
    Quarterly = 4,      // Quarterly budget
    SemiAnnual = 5,     // Every 6 months
    Annual = 6,         // Annual budget
    Custom = 7          // Custom date range
}
```

### 3.5 ScheduledTransactions Table

```sql
CREATE TABLE ScheduledTransactions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name NVARCHAR(100) NOT NULL,
    AccountId INTEGER NOT NULL,
    TransactionType INTEGER NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    CategoryId INTEGER NULL,
    PayeePayerName NVARCHAR(200) NULL,
    Description NVARCHAR(500) NULL,
    RecurrenceType INTEGER NOT NULL,
    RecurrenceInterval INTEGER NOT NULL DEFAULT 1,
    RecurrenceDay INTEGER NULL,
    NextDueDate DATE NOT NULL,
    EndDate DATE NULL,
    MaxOccurrences INTEGER NULL,
    OccurrenceCount INTEGER NOT NULL DEFAULT 0,
    AutoPost BOOLEAN NOT NULL DEFAULT 0,
    IsActive BOOLEAN NOT NULL DEFAULT 1,
    LastProcessedDate DATE NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE SET NULL,
    
    CONSTRAINT CK_ScheduledTransactions_Amount CHECK (Amount != 0),
    CONSTRAINT CK_ScheduledTransactions_Interval CHECK (RecurrenceInterval > 0),
    CONSTRAINT CK_ScheduledTransactions_MaxOccurrences CHECK (MaxOccurrences IS NULL OR MaxOccurrences > 0)
);
```

**Recurrence Types Enum:**
```csharp
public enum RecurrenceType
{
    Daily = 1,          // Every N days
    Weekly = 2,         // Every N weeks on specific day
    Monthly = 3,        // Every N months on specific date
    Quarterly = 4,      // Every N quarters
    SemiAnnual = 5,     // Every N half-years
    Annual = 6,         // Every N years
    Custom = 7          // Custom pattern
}
```

---

## 4. Supporting Entities

### 4.1 TransactionTags Table

```sql
CREATE TABLE TransactionTags (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TransactionId INTEGER NOT NULL,
    TagName NVARCHAR(50) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (TransactionId) REFERENCES Transactions(Id) ON DELETE CASCADE,
    
    CONSTRAINT UQ_TransactionTags_TransactionTag UNIQUE (TransactionId, TagName),
    CONSTRAINT CK_TransactionTags_TagName CHECK (LENGTH(TRIM(TagName)) > 0)
);
```

### 4.2 Attachments Table

```sql
CREATE TABLE Attachments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ParentType INTEGER NOT NULL,
    ParentId INTEGER NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    FileSize INTEGER NOT NULL,
    MimeType NVARCHAR(100) NOT NULL,
    ThumbnailPath NVARCHAR(500) NULL,
    Description NVARCHAR(200) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT CK_Attachments_FileSize CHECK (FileSize > 0),
    CONSTRAINT CK_Attachments_FileName CHECK (LENGTH(TRIM(FileName)) > 0)
);
```

**Attachment Parent Types Enum:**
```csharp
public enum AttachmentParentType
{
    Transaction = 1,    // Transaction receipt/document
    Asset = 2,          // Asset documentation
    Debt = 3,           // Debt/loan documentation
    Account = 4         // Account documentation
}
```

### 4.3 Debts Table

```sql
CREATE TABLE Debts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CreditorName NVARCHAR(200) NOT NULL,
    DebtType NVARCHAR(50) NULL,
    AccountNumber NVARCHAR(50) NULL,
    OriginalAmount DECIMAL(18,2) NOT NULL,
    CurrentBalance DECIMAL(18,2) NOT NULL,
    InterestRate DECIMAL(7,4) NULL,
    MinimumPayment DECIMAL(18,2) NULL,
    PaymentDueDay INTEGER NULL,
    PaymentFrequency INTEGER NOT NULL DEFAULT 3,
    AccountId INTEGER NULL,
    NextPaymentDate DATE NULL,
    IsActive BOOLEAN NOT NULL DEFAULT 1,
    Notes NVARCHAR(500) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE SET NULL,
    
    CONSTRAINT CK_Debts_OriginalAmount CHECK (OriginalAmount > 0),
    CONSTRAINT CK_Debts_CurrentBalance CHECK (CurrentBalance >= 0),
    CONSTRAINT CK_Debts_InterestRate CHECK (InterestRate IS NULL OR InterestRate >= 0),
    CONSTRAINT CK_Debts_PaymentDueDay CHECK (PaymentDueDay IS NULL OR PaymentDueDay BETWEEN 1 AND 31)
);
```

### 4.4 Assets Table

```sql
CREATE TABLE Assets (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name NVARCHAR(200) NOT NULL,
    Category NVARCHAR(100) NULL,
    SerialNumber NVARCHAR(100) NULL,
    PurchaseDate DATE NULL,
    PurchasePrice DECIMAL(18,2) NULL,
    CurrentValue DECIMAL(18,2) NULL,
    DepreciationMethod INTEGER NOT NULL DEFAULT 0,
    DepreciationRate DECIMAL(7,4) NULL,
    UsefulLife INTEGER NULL,
    PurchaseTransactionId INTEGER NULL,
    InsurancePolicyNumber NVARCHAR(100) NULL,
    InsuranceValue DECIMAL(18,2) NULL,
    IsActive BOOLEAN NOT NULL DEFAULT 1,
    Notes NVARCHAR(500) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (PurchaseTransactionId) REFERENCES Transactions(Id) ON DELETE SET NULL,
    
    CONSTRAINT CK_Assets_PurchasePrice CHECK (PurchasePrice IS NULL OR PurchasePrice >= 0),
    CONSTRAINT CK_Assets_CurrentValue CHECK (CurrentValue IS NULL OR CurrentValue >= 0),
    CONSTRAINT CK_Assets_UsefulLife CHECK (UsefulLife IS NULL OR UsefulLife > 0)
);
```

**Depreciation Methods Enum:**
```csharp
public enum DepreciationMethod
{
    None = 0,           // No depreciation
    StraightLine = 1,   // Straight-line depreciation
    DecliningBalance = 2, // Declining balance method
    Custom = 3          // Custom depreciation schedule
}
```

### 4.5 Notifications Table

```sql
CREATE TABLE Notifications (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title NVARCHAR(100) NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    NotificationType INTEGER NOT NULL,
    Priority INTEGER NOT NULL DEFAULT 2,
    DueDate DATETIME NOT NULL,
    IsRead BOOLEAN NOT NULL DEFAULT 0,
    IsProcessed BOOLEAN NOT NULL DEFAULT 0,
    RelatedEntityType INTEGER NULL,
    RelatedEntityId INTEGER NULL,
    ActionData NVARCHAR(1000) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ReadDate DATETIME NULL,
    
    CONSTRAINT CK_Notifications_Title CHECK (LENGTH(TRIM(Title)) > 0),
    CONSTRAINT CK_Notifications_Message CHECK (LENGTH(TRIM(Message)) > 0)
);
```

**Notification Types Enum:**
```csharp
public enum NotificationType
{
    Reminder = 1,       // General reminder
    BillDue = 2,        // Bill payment due
    BudgetAlert = 3,    // Budget threshold alert
    DebtPayment = 4,    // Debt payment reminder
    IncomeExpected = 5, // Expected income
    Reconciliation = 6, // Reconciliation reminder
    BackupReminder = 7  // Backup reminder
}
```

### 4.6 Insurance Table

```sql
CREATE TABLE Insurance (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PolicyNumber NVARCHAR(100) NOT NULL,
    InsuranceType INTEGER NOT NULL,
    ProviderName NVARCHAR(200) NOT NULL,
    PolicyHolderName NVARCHAR(200) NOT NULL,
    CoverageAmount DECIMAL(18,2) NOT NULL,
    PremiumAmount DECIMAL(18,2) NOT NULL,
    PremiumFrequency INTEGER NOT NULL DEFAULT 3,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    DeductibleAmount DECIMAL(18,2) NULL,
    BeneficiaryName NVARCHAR(200) NULL,
    AgentName NVARCHAR(200) NULL,
    AgentContact NVARCHAR(100) NULL,
    IsActive BOOLEAN NOT NULL DEFAULT 1,
    Notes NVARCHAR(500) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT CK_Insurance_CoverageAmount CHECK (CoverageAmount > 0),
    CONSTRAINT CK_Insurance_PremiumAmount CHECK (PremiumAmount > 0),
    CONSTRAINT CK_Insurance_DeductibleAmount CHECK (DeductibleAmount IS NULL OR DeductibleAmount >= 0),
    CONSTRAINT CK_Insurance_PolicyNumber CHECK (LENGTH(TRIM(PolicyNumber)) > 0),
    CONSTRAINT CK_Insurance_DateRange CHECK (EndDate > StartDate)
);
```

**Insurance Types Enum:**
```csharp
public enum InsuranceType
{
    Life = 1,           // Life insurance
    Health = 2,         // Health/medical insurance
    Auto = 3,           // Vehicle insurance
    Home = 4,           // Home/property insurance
    Disability = 5,     // Disability insurance
    Travel = 6,         // Travel insurance
    Business = 7,       // Business insurance
    Other = 8           // Other insurance types
}
```

### 4.7 InsuranceClaim Table

```sql
CREATE TABLE InsuranceClaim (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    InsuranceId INTEGER NOT NULL,
    ClaimNumber NVARCHAR(100) NOT NULL,
    ClaimDate DATE NOT NULL,
    IncidentDate DATE NOT NULL,
    ClaimAmount DECIMAL(18,2) NOT NULL,
    ApprovedAmount DECIMAL(18,2) NULL,
    ClaimStatus INTEGER NOT NULL DEFAULT 1,
    Description NVARCHAR(1000) NOT NULL,
    AdjusterName NVARCHAR(200) NULL,
    AdjusterContact NVARCHAR(100) NULL,
    SettlementDate DATE NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (InsuranceId) REFERENCES Insurance(Id) ON DELETE CASCADE,
    
    CONSTRAINT CK_InsuranceClaim_ClaimAmount CHECK (ClaimAmount > 0),
    CONSTRAINT CK_InsuranceClaim_ApprovedAmount CHECK (ApprovedAmount IS NULL OR ApprovedAmount >= 0),
    CONSTRAINT CK_InsuranceClaim_ClaimNumber CHECK (LENGTH(TRIM(ClaimNumber)) > 0),
    CONSTRAINT CK_InsuranceClaim_DateRange CHECK (ClaimDate >= IncidentDate)
);
```

**Claim Status Enum:**
```csharp
public enum ClaimStatus
{
    Submitted = 1,      // Claim submitted
    UnderReview = 2,    // Under review
    Approved = 3,       // Claim approved
    Denied = 4,         // Claim denied
    Settled = 5,        // Claim settled
    Closed = 6          // Claim closed
}
```

### 4.8 Obligation Table

```sql
CREATE TABLE Obligation (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name NVARCHAR(200) NOT NULL,
    ObligationType INTEGER NOT NULL,
    GroupName NVARCHAR(200) NULL,
    ContributionAmount DECIMAL(18,2) NOT NULL,
    ContributionFrequency INTEGER NOT NULL DEFAULT 3,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    TotalContributed DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    ExpectedReturn DECIMAL(18,2) NULL,
    InterestRate DECIMAL(7,4) NULL,
    MembershipNumber NVARCHAR(100) NULL,
    ContactPerson NVARCHAR(200) NULL,
    ContactPhone NVARCHAR(50) NULL,
    ContactEmail NVARCHAR(200) NULL,
    MeetingSchedule NVARCHAR(200) NULL,
    IsActive BOOLEAN NOT NULL DEFAULT 1,
    Notes NVARCHAR(1000) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT CK_Obligation_ContributionAmount CHECK (ContributionAmount > 0),
    CONSTRAINT CK_Obligation_TotalContributed CHECK (TotalContributed >= 0),
    CONSTRAINT CK_Obligation_ExpectedReturn CHECK (ExpectedReturn IS NULL OR ExpectedReturn >= 0),
    CONSTRAINT CK_Obligation_Name CHECK (LENGTH(TRIM(Name)) > 0)
);
```

**Obligation Types Enum:**
```csharp
public enum ObligationType
{
    Chama = 1,          // Investment group/chama
    SACCO = 2,          // Savings and Credit Cooperative
    Welfare = 3,        // Welfare group
    Merry_Go_Round = 4, // Rotating savings group
    Investment_Club = 5, // Investment club
    Burial_Society = 6, // Burial/funeral society
    Other = 7           // Other group obligations
}
```

### 4.9 ObligationPayment Table

```sql
CREATE TABLE ObligationPayment (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ObligationId INTEGER NOT NULL,
    PaymentDate DATE NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PaymentType INTEGER NOT NULL DEFAULT 1,
    TransactionId INTEGER NULL,
    ReceiptNumber NVARCHAR(100) NULL,
    PaymentMethod NVARCHAR(50) NULL,
    Notes NVARCHAR(500) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (ObligationId) REFERENCES Obligation(Id) ON DELETE CASCADE,
    FOREIGN KEY (TransactionId) REFERENCES Transactions(Id) ON DELETE SET NULL,
    
    CONSTRAINT CK_ObligationPayment_Amount CHECK (Amount > 0)
);
```

**Payment Types Enum:**
```csharp
public enum PaymentType
{
    Regular = 1,        // Regular contribution
    Penalty = 2,        // Penalty payment
    Loan_Repayment = 3, // Loan repayment
    Share_Purchase = 4, // Share purchase
    Special_Levy = 5,   // Special assessment
    Dividend = 6,       // Dividend payment
    Other = 7           // Other payment types
}
```

---

## 5. Database Indexes

### 5.1 Primary Indexes (Performance Critical)

```sql
-- Transaction queries (most frequent)
CREATE INDEX IX_Transactions_AccountId_Date ON Transactions(AccountId, TransactionDate DESC);
CREATE INDEX IX_Transactions_CategoryId_Date ON Transactions(CategoryId, TransactionDate DESC);
CREATE INDEX IX_Transactions_Date_Type ON Transactions(TransactionDate DESC, TransactionType);
CREATE INDEX IX_Transactions_Status ON Transactions(Status);

-- Account balance calculations
CREATE INDEX IX_Transactions_Account_Status ON Transactions(AccountId, Status, TransactionDate);

-- Budget tracking
CREATE INDEX IX_Transactions_Category_Date_Amount ON Transactions(CategoryId, TransactionDate, Amount);

-- Search and filtering
CREATE INDEX IX_Transactions_PayeePayer ON Transactions(PayeePayerName);
CREATE INDEX IX_Transactions_Description ON Transactions(Description);
```

### 5.2 Secondary Indexes (Supporting Queries)

```sql
-- Scheduled transactions
CREATE INDEX IX_ScheduledTransactions_NextDue ON ScheduledTransactions(NextDueDate, IsActive);
CREATE INDEX IX_ScheduledTransactions_Account ON ScheduledTransactions(AccountId, IsActive);

-- Categories
CREATE INDEX IX_Categories_Parent_Type ON Categories(ParentCategoryId, CategoryType);
CREATE INDEX IX_Categories_Type_Active ON Categories(CategoryType, IsActive);

-- Budgets
CREATE INDEX IX_Budgets_Category_Period ON Budgets(CategoryId, PeriodType, IsActive);
CREATE INDEX IX_Budgets_DateRange ON Budgets(StartDate, EndDate, IsActive);

-- Tags
CREATE INDEX IX_TransactionTags_Tag ON TransactionTags(TagName);
CREATE INDEX IX_TransactionTags_Transaction ON TransactionTags(TransactionId);

-- Notifications
CREATE INDEX IX_Notifications_DueDate_Read ON Notifications(DueDate, IsRead);
CREATE INDEX IX_Notifications_Type_Priority ON Notifications(NotificationType, Priority);

-- Attachments
CREATE INDEX IX_Attachments_Parent ON Attachments(ParentType, ParentId);

-- Debts
CREATE INDEX IX_Debts_NextPayment ON Debts(NextPaymentDate, IsActive);

-- Assets
CREATE INDEX IX_Assets_Category_Active ON Assets(Category, IsActive);
```

### 5.3 Composite Indexes (Complex Queries)

```sql
-- Reporting queries
CREATE INDEX IX_Transactions_Reporting ON Transactions(TransactionDate, TransactionType, CategoryId, Amount);

-- Budget analysis
CREATE INDEX IX_Budget_Analysis ON Transactions(CategoryId, TransactionDate, TransactionType, Amount);

-- Account reconciliation
CREATE INDEX IX_Reconciliation ON Transactions(AccountId, IsReconciled, TransactionDate, Status);
```

---

## 6. Entity Framework Models

### 6.1 Account Model

```csharp
public class Account
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    public AccountType AccountType { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "USD";
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal OpeningBalance { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentBalance { get; set; }
    
    [MaxLength(7)]
    public string? ColorCode { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int SortOrder { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Transaction> TransferTransactions { get; set; } = new List<Transaction>();
    public virtual ICollection<ScheduledTransaction> ScheduledTransactions { get; set; } = new List<ScheduledTransaction>();
    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public virtual ICollection<Debt> Debts { get; set; } = new List<Debt>();
}
```

### 6.2 Transaction Model

```csharp
public class Transaction
{
    public int Id { get; set; }
    
    public int AccountId { get; set; }
    
    public TransactionType TransactionType { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    public DateTime TransactionDate { get; set; }
    
    public int? CategoryId { get; set; }
    
    [MaxLength(200)]
    public string? PayeePayerName { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    
    public int? TransferAccountId { get; set; }
    
    public int? ParentTransactionId { get; set; }
    
    [MaxLength(20)]
    public string? CheckNumber { get; set; }
    
    [MaxLength(50)]
    public string? ReferenceNumber { get; set; }
    
    public bool IsReconciled { get; set; }
    
    public DateTime? ReconciliationDate { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Account Account { get; set; }
    public virtual Account? TransferAccount { get; set; }
    public virtual Category? Category { get; set; }
    public virtual Transaction? ParentTransaction { get; set; }
    public virtual ICollection<Transaction> SplitTransactions { get; set; } = new List<Transaction>();
    public virtual ICollection<TransactionTag> Tags { get; set; } = new List<TransactionTag>();
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
```

### 6.3 Category Model

```csharp
public class Category
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    public int? ParentCategoryId { get; set; }
    
    public CategoryType CategoryType { get; set; }
    
    [MaxLength(7)]
    public string? ColorCode { get; set; }
    
    [MaxLength(50)]
    public string? IconName { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsSystemCategory { get; set; }
    
    public int SortOrder { get; set; }
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public virtual ICollection<ScheduledTransaction> ScheduledTransactions { get; set; } = new List<ScheduledTransaction>();
}
```

---

## 7. Database Migrations

### 7.1 Initial Migration (Version 1.0)

```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create Accounts table
        migrationBuilder.CreateTable(
            name: "Accounts",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                AccountType = table.Column<int>(type: "INTEGER", nullable: false),
                Currency = table.Column<string>(type: "NVARCHAR(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                ColorCode = table.Column<string>(type: "NVARCHAR(7)", maxLength: 7, nullable: true),
                IsActive = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: true),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                Notes = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                CreatedDate = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                ModifiedDate = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Accounts", x => x.Id);
                table.CheckConstraint("CK_Accounts_Currency", "LENGTH(Currency) = 3");
                table.CheckConstraint("CK_Accounts_ColorCode", "ColorCode IS NULL OR ColorCode LIKE '#%'");
                table.CheckConstraint("CK_Accounts_Name", "LENGTH(TRIM(Name)) > 0");
            });

        // Create Categories table
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                ParentCategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                CategoryType = table.Column<int>(type: "INTEGER", nullable: false),
                ColorCode = table.Column<string>(type: "NVARCHAR(7)", maxLength: 7, nullable: true),
                IconName = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: true),
                IsActive = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: true),
                IsSystemCategory = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                Description = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: true),
                CreatedDate = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                ModifiedDate = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
                table.ForeignKey(
                    name: "FK_Categories_Categories_ParentCategoryId",
                    column: x => x.ParentCategoryId,
                    principalTable: "Categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.CheckConstraint("CK_Categories_Name", "LENGTH(TRIM(Name)) > 0");
                table.CheckConstraint("CK_Categories_ColorCode", "ColorCode IS NULL OR ColorCode LIKE '#%'");
            });

        // Continue with other tables...
        // (Additional table creation code would follow)
        
        // Create indexes
        migrationBuilder.CreateIndex(
            name: "IX_Transactions_AccountId_Date",
            table: "Transactions",
            columns: new[] { "AccountId", "TransactionDate" });
            
        // (Additional index creation code would follow)
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("Transactions");
        migrationBuilder.DropTable("Categories");
        migrationBuilder.DropTable("Accounts");
        // (Drop other tables in reverse order)
    }
}
```

### 7.2 Seed Data Migration

```csharp
public partial class SeedDefaultData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Insert default categories
        migrationBuilder.InsertData(
            table: "Categories",
            columns: new[] { "Name", "CategoryType", "IsSystemCategory", "SortOrder", "ColorCode", "IconName" },
            values: new object[,]
            {
                { "Income", 1, true, 1, "#4CAF50", "income" },
                { "Salary", 1, true, 2, "#4CAF50", "salary" },
                { "Freelance", 1, true, 3, "#4CAF50", "freelance" },
                { "Investment Income", 1, true, 4, "#4CAF50", "investment" },
                { "Other Income", 1, true, 5, "#4CAF50", "other" },
                
                { "Expenses", 2, true, 10, "#F44336", "expense" },
                { "Food & Dining", 2, true, 11, "#FF9800", "food" },
                { "Transportation", 2, true, 12, "#2196F3", "transport" },
                { "Shopping", 2, true, 13, "#9C27B0", "shopping" },
                { "Entertainment", 2, true, 14, "#E91E63", "entertainment" },
                { "Bills & Utilities", 2, true, 15, "#607D8B", "bills" },
                { "Healthcare", 2, true, 16, "#009688", "healthcare" },
                { "Education", 2, true, 17, "#795548", "education" },
                { "Travel", 2, true, 18, "#FF5722", "travel" },
                { "Other Expenses", 2, true, 19, "#757575", "other" }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "IsSystemCategory",
            keyValue: true);
    }
}
```

---

## 8. Data Validation Rules

### 8.1 Business Rules

#### 8.1.1 Account Rules
- Account names must be unique within the same account type
- Opening balance cannot be modified after transactions exist
- Account cannot be deleted if transactions exist (soft delete only)
- Currency code must be valid ISO 4217 format
- Color code must be valid hex format (#RRGGBB)

#### 8.1.2 Transaction Rules
- Transaction amount cannot be zero
- Transfer transactions must have both source and destination accounts
- Transfer transactions cannot be to the same account
- Split transactions must sum to parent transaction amount
- Transaction date cannot be more than 1 year in the future
- Reconciled transactions cannot be modified

#### 8.1.3 Budget Rules
- Budget amount must be positive
- Budget period must have valid start and end dates
- Alert threshold must be between 0 and 100 percent
- Budget cannot overlap with existing budget for same category/period
- Rollover amount cannot exceed unused budget amount

#### 8.1.4 Category Rules
- Category names must be unique within the same parent category
- Category hierarchy cannot exceed 5 levels deep
- System categories cannot be deleted or have type changed
- Parent category must be of same type (income/expense)
- Cannot create circular references in category hierarchy

### 8.2 Data Integrity Constraints

#### 8.2.1 Referential Integrity
- All foreign key relationships enforced at database level
- Cascade deletes only for dependent entities (tags, attachments)
- Set null for optional relationships when parent deleted
- Restrict deletes for core entities with dependencies

#### 8.2.2 Data Consistency
- Account balances automatically calculated from transactions
- Budget utilization calculated from related transactions
- Debt balances updated when payments recorded
- Asset values updated based on depreciation schedules

### 8.3 Validation Implementation

```csharp
public class TransactionValidator : AbstractValidator<Transaction>
{
    public TransactionValidator()
    {
        RuleFor(t => t.Amount)
            .NotEqual(0)
            .WithMessage("Transaction amount cannot be zero");
            
        RuleFor(t => t.TransactionDate)
            .LessThanOrEqualTo(DateTime.Today.AddYears(1))
            .WithMessage("Transaction date cannot be more than 1 year in the future");
            
        RuleFor(t => t.AccountId)
            .NotEqual(t => t.TransferAccountId)
            .When(t => t.TransactionType == TransactionType.Transfer)
            .WithMessage("Transfer cannot be to the same account");
            
        RuleFor(t => t.TransferAccountId)
            .NotNull()
            .When(t => t.TransactionType == TransactionType.Transfer)
            .WithMessage("Transfer account is required for transfer transactions");
    }
}
```

---

## Document Control

**Document Version:** 1.0  
**Last Updated:** January 2025  
**Next Review Date:** March 2025  
**Related Documents:** SystemRequirements.md

### Change Log
| Version | Date | Changes |
|---------|------|---------|
| 1.0 | January 2025 | Initial data model specification |