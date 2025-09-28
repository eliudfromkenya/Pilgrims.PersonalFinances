using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Tests.Data;

public class PersonalFinanceContextTests : IDisposable
{
    private readonly PersonalFinanceContext _context;

    public PersonalFinanceContextTests()
    {
        var options = new DbContextOptionsBuilder<PersonalFinanceContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PersonalFinanceContext(options);
    }

    [Fact]
    public async Task Context_ShouldCreateDatabase_WhenEnsureCreatedCalled()
    {
        // Act
        var result = await _context.Database.EnsureCreatedAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Accounts_ShouldSaveAndRetrieve_WhenValidAccountAdded()
    {
        // Arrange
        var account = new Account
        {
            Name = "Test Account",
            AccountType = AccountType.Checking,
            InitialBalance = 1000.00m,
            Currency = "USD",
            IsActive = true
        };

        // Act
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        var retrievedAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Name == "Test Account");

        // Assert
        retrievedAccount.Should().NotBeNull();
        retrievedAccount!.Name.Should().Be("Test Account");
        retrievedAccount.AccountType.Should().Be(AccountType.Checking);
        retrievedAccount.Balance.Should().Be(1000.00m);
    }

    [Fact]
    public async Task Transactions_ShouldSaveAndRetrieve_WhenValidTransactionAdded()
    {
        // Arrange
        var account = new Account
        {
            Name = "Test Account",
            AccountType = AccountType.Checking,
            InitialBalance = 1000.00m,
            Currency = "USD"
        };
        
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        var transaction = new Transaction
        {
            Amount = 100.00m,
            Date = DateTime.Today,
            Type = TransactionType.Expense,
            AccountId = account.Id,
            Description = "Test Transaction"
        };

        // Act
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        var retrievedTransaction = await _context.Transactions
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Description == "Test Transaction");

        // Assert
        retrievedTransaction.Should().NotBeNull();
        retrievedTransaction!.Amount.Should().Be(100.00m);
        retrievedTransaction.Type.Should().Be(TransactionType.Expense);
        retrievedTransaction.Account.Should().NotBeNull();
        retrievedTransaction.Account!.Name.Should().Be("Test Account");
    }

    [Fact]
    public async Task Categories_ShouldSaveAndRetrieve_WhenValidCategoryAdded()
    {
        // Arrange
        var category = new Category
        {
            Name = "Food & Dining",
            Description = "Expenses for food and dining out",
            ColorCode = "#FF5722",
            IsActive = true
        };

        // Act
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var retrievedCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Food & Dining");

        // Assert
        retrievedCategory.Should().NotBeNull();
        retrievedCategory!.Name.Should().Be("Food & Dining");
        retrievedCategory.Description.Should().Be("Expenses for food and dining out");
        retrievedCategory.ColorCode.Should().Be("#FF5722");
    }

    [Fact]
    public async Task Budgets_ShouldSaveAndRetrieve_WhenValidBudgetAdded()
    {
        // Arrange
        var budget = new Budget
        {
            Name = "Monthly Budget",
            LimitAmount = 2000.00m,
            Period = BudgetPeriod.Monthly,
            BudgetType = BudgetType.Category,
            StartDate = DateTime.Today.AddDays(-30),
            EndDate = DateTime.Today,
            IsActive = true
        };

        // Act
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        var retrievedBudget = await _context.Budgets.FirstOrDefaultAsync(b => b.Name == "Monthly Budget");

        // Assert
        retrievedBudget.Should().NotBeNull();
        retrievedBudget!.Name.Should().Be("Monthly Budget");
        retrievedBudget.LimitAmount.Should().Be(2000.00m);
        retrievedBudget.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Goals_ShouldSaveAndRetrieve_WhenValidGoalAdded()
    {
        // Arrange
        var goal = new Goal
        {
            Name = "Emergency Fund",
            Description = "Build emergency fund for 6 months expenses",
            TargetAmount = 10000.00m,
            CurrentAmount = 2500.00m,
            TargetDate = DateTime.Today.AddMonths(12),
            IsActive = true
        };

        // Act
        _context.Goals.Add(goal);
        await _context.SaveChangesAsync();

        var retrievedGoal = await _context.Goals.FirstOrDefaultAsync(g => g.Name == "Emergency Fund");

        // Assert
        retrievedGoal.Should().NotBeNull();
        retrievedGoal!.Name.Should().Be("Emergency Fund");
        retrievedGoal.TargetAmount.Should().Be(10000.00m);
        retrievedGoal.CurrentAmount.Should().Be(2500.00m);
    }

    [Fact]
    public async Task Users_ShouldSaveAndRetrieve_WhenValidUserAdded()
    {
        // Arrange
        var user = new User
        {
            Id = "user1",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hashedpassword",
            PasswordSalt = "salt"
        };

        // Act
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var retrievedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");

        // Assert
        retrievedUser.Should().NotBeNull();
        retrievedUser!.Email.Should().Be("test@example.com");
        retrievedUser.Email.Should().Be("test@example.com");
        retrievedUser.FirstName.Should().Be("John");
        retrievedUser.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task Incomes_ShouldSaveAndRetrieve_WhenValidIncomeAdded()
    {
        // Arrange
        var income = new Income
        {
            Source = "Salary",
            Amount = 5000.00m,
            Frequency = "Monthly",
            StartDate = DateTime.Today.AddMonths(-1),
            IsActive = true
        };

        // Act
        _context.Incomes.Add(income);
        await _context.SaveChangesAsync();

        var retrievedIncome = await _context.Incomes.FirstOrDefaultAsync(i => i.Source == "Salary");

        // Assert
        retrievedIncome.Should().NotBeNull();
        retrievedIncome!.Source.Should().Be("Salary");
        retrievedIncome.Amount.Should().Be(5000.00m);
        retrievedIncome.Frequency.Should().Be("Monthly");
    }

    [Fact]
    public async Task Debts_ShouldSaveAndRetrieve_WhenValidDebtAdded()
    {
        // Arrange
        var debt = new Debt
        {
            Name = "Credit Card Debt",
            PrincipalAmount = 5000.00m,
            CurrentBalance = 3500.00m,
            InterestRate = 18.99m,
            MinimumPayment = 150.00m,
            IsActive = true
        };

        // Act
        _context.Debts.Add(debt);
        await _context.SaveChangesAsync();

        var retrievedDebt = await _context.Debts.FirstOrDefaultAsync(d => d.Name == "Credit Card Debt");

        // Assert
        retrievedDebt.Should().NotBeNull();
        retrievedDebt!.Name.Should().Be("Credit Card Debt");
        retrievedDebt.PrincipalAmount.Should().Be(5000.00m);
        retrievedDebt.CurrentBalance.Should().Be(3500.00m);
        retrievedDebt.InterestRate.Should().Be(18.99m);
    }

    [Fact]
    public async Task Assets_ShouldSaveAndRetrieve_WhenValidAssetAdded()
    {
        // Arrange
        var asset = new Asset
        {
            Name = "Primary Residence",
            Description = "Main family home",
            CurrentValue = 350000.00m,
            PurchasePrice = 300000.00m,
            PurchaseDate = DateTime.Today.AddYears(-5),
            IsActive = true
        };

        // Act
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        var retrievedAsset = await _context.Assets.FirstOrDefaultAsync(a => a.Name == "Primary Residence");

        // Assert
        retrievedAsset.Should().NotBeNull();
        retrievedAsset!.Name.Should().Be("Primary Residence");
        retrievedAsset.CurrentValue.Should().Be(350000.00m);
        retrievedAsset.PurchasePrice.Should().Be(300000.00m);
    }

    [Fact]
    public async Task Context_ShouldHandleConcurrentOperations_WhenMultipleEntitiesAdded()
    {
        // Arrange
        var account = new Account { Name = "Test Account", AccountType = AccountType.Checking, InitialBalance = 1000m };
        var category = new Category { Name = "Test Category", ColorCode = "#FF5722", IsActive = true };
        var budget = new Budget { Name = "Test Budget", LimitAmount = 500m, Period = BudgetPeriod.Monthly, BudgetType = BudgetType.Category };

        // Act
        _context.Accounts.Add(account);
        _context.Categories.Add(category);
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        // Assert
        var accountCount = await _context.Accounts.CountAsync();
        var categoryCount = await _context.Categories.CountAsync();
        var budgetCount = await _context.Budgets.CountAsync();

        accountCount.Should().Be(1);
        categoryCount.Should().Be(1);
        budgetCount.Should().Be(1);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}