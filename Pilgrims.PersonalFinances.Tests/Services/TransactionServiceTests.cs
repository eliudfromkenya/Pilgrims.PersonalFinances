using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Core.Models.Enums;
using Pilgrims.PersonalFinances.Core.Services;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using Moq;
using Pilgrims.PersonalFinances.Core.Interfaces;
using Pilgrims.PersonalFinances.Core.Logging;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Pilgrims.PersonalFinances.Core;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Tests.Utilities;

namespace Pilgrims.PersonalFinances.Tests.Services;

public class TransactionServiceTests : IDisposable
{
    private readonly PersonalFinanceContext _context;
    private readonly TransactionService _transactionService;
    private readonly Account _testAccount;

    public TransactionServiceTests()
    {
        // Initialize GlobalExtensions with test DI so SaveChanges can generate IDs
        var services = new ServiceCollection();
        services.AddSingleton<IMessagingService, TestMessagingService>();
        services.AddSingleton<IIdGenerator, TestIdGenerator>();
        GlobalExtensions.InitApp(services);

        var dbPath = Path.Combine(Path.GetTempPath(), $"pf-tests-{Guid.NewGuid()}.db");
        var options = new DbContextOptionsBuilder<PersonalFinanceContext>()
            .UseSqlite($"Data Source={dbPath};")
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .LogTo(Console.WriteLine)
            .Options;

        _context = new PersonalFinanceContext(options);
        _context.Database.EnsureCreated();
        var mockLogger = new Mock<ILoggingService>();
        var mockCurrencyService = new Mock<ICurrencyService>();
        mockCurrencyService.Setup(x => x.FormatAmount(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<bool>())).Returns("$0.00");
        var provider = new SqliteConnectionProvider(dbPath);
        var sqliteWrite = new SqliteWriteService(provider);
        _transactionService = new TransactionService(_context, mockLogger.Object, mockCurrencyService.Object, sqliteWrite);

        // Create a dummy user (in case of implicit FK usage) and a test account
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash",
            PasswordSalt = "salt",
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        _testAccount = new Account
        {
            Name = "Test Account",
            AccountType = AccountType.Checking,
            InitialBalance = 1000m,
            CurrentBalance = 1000m,
            Currency = "USD"
        };
        _testAccount.UserId = user.Id;
        
        _context.Accounts.Add(_testAccount);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateTransactionAsync_ShouldCreateTransaction_WhenValidDataProvided()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = 100.00m,
            Date = DateTime.Today,
            Type = TransactionType.Expense,
            AccountId = _testAccount.Id,
            Description = "Test Transaction"
        };

        // Act
        var result = await _transactionService.CreateTransactionAsync(transaction);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Amount.Should().Be(100.00m);
        result.Type.Should().Be(TransactionType.Expense);
        result.Description.Should().Be("Test Transaction");
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ShouldReturnTransaction_WhenTransactionExists()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = 250.00m,
            Date = DateTime.Today,
            Type = TransactionType.Income,
            AccountId = _testAccount.Id,
            Description = "Income Transaction"
        };
        
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.GetTransactionByIdAsync(transaction.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(transaction.Id);
        result.Amount.Should().Be(250.00m);
        result.Type.Should().Be(TransactionType.Income);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ShouldReturnNull_WhenTransactionDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid().ToString();

        // Act
        var result = await _transactionService.GetTransactionByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTransactionsByAccountIdAsync_ShouldReturnAccountTransactions()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new() { Amount = 100m, Date = DateTime.Today, Type = TransactionType.Expense, AccountId = _testAccount.Id, Description = "Transaction 1" },
            new() { Amount = 200m, Date = DateTime.Today, Type = TransactionType.Income, AccountId = _testAccount.Id, Description = "Transaction 2" },
            new() { Amount = 50m, Date = DateTime.Today, Type = TransactionType.Expense, AccountId = _testAccount.Id, Description = "Transaction 3" }
        };

        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.GetTransactionsByAccountAsync(_testAccount.Id);

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(t => t.AccountId == _testAccount.Id);
    }

    [Fact]
    public async Task GetTransactionsByDateRangeAsync_ShouldReturnTransactionsInRange()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-7);
        var endDate = DateTime.Today;
        
        var transactions = new List<Transaction>
        {
            new() { Amount = 100m, Date = DateTime.Today.AddDays(-5), Type = TransactionType.Expense, AccountId = _testAccount.Id, Description = "In Range 1" },
            new() { Amount = 200m, Date = DateTime.Today.AddDays(-3), Type = TransactionType.Income, AccountId = _testAccount.Id, Description = "In Range 2" },
            new() { Amount = 50m, Date = DateTime.Today.AddDays(-10), Type = TransactionType.Expense, AccountId = _testAccount.Id, Description = "Out of Range" }
        };

        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.GetTransactionsByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.Date >= startDate && t.Date <= endDate);
    }

    [Theory]
    [InlineData(TransactionType.Income)]
    [InlineData(TransactionType.Expense)]
    [InlineData(TransactionType.Transfer)]
    public async Task GetTransactionsByTypeAsync_ShouldReturnTransactionsOfSpecificType(TransactionType transactionType)
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new() { Amount = 100m, Date = DateTime.Today, Type = TransactionType.Income, AccountId = _testAccount.Id, Description = "Income" },
            new() { Amount = 200m, Date = DateTime.Today, Type = TransactionType.Expense, AccountId = _testAccount.Id, Description = "Expense" },
            new() { Amount = 50m, Date = DateTime.Today, Type = TransactionType.Transfer, AccountId = _testAccount.Id, Description = "Transfer" }
        };

        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.GetTransactionsByTypeAsync(transactionType);

        // Assert
        result.Should().HaveCount(1);
        result.First().Type.Should().Be(transactionType);
    }

    [Fact]
    public async Task UpdateTransactionAsync_ShouldUpdateTransaction_WhenValidDataProvided()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = 100m,
            Date = DateTime.Today,
            Type = TransactionType.Expense,
            AccountId = _testAccount.Id,
            Description = "Original Description"
        };
        
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        transaction.Amount = 150m;
        transaction.Description = "Updated Description";

        // Act
        var result = await _transactionService.UpdateTransactionAsync(transaction);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(150m);
        result.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task DeleteTransactionAsync_ShouldDeleteTransaction_WhenTransactionExists()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = 100m,
            Date = DateTime.Today,
            Type = TransactionType.Expense,
            AccountId = _testAccount.Id,
            Description = "Transaction to Delete"
        };
        
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.DeleteTransactionAsync(transaction.Id);

        // Assert
        result.Should().BeTrue();
        
        var deletedTransaction = await _context.Transactions.FindAsync(transaction.Id);
        deletedTransaction.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTransactionAsync_ShouldReturnFalse_WhenTransactionDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid().ToString();

        // Act
        var result = await _transactionService.DeleteTransactionAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetTotalIncomeAsync_ShouldReturnCorrectSum()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-30);
        var endDate = DateTime.Today;
        var transactions = new List<Transaction>
        {
            new() { Amount = 1000m, Date = DateTime.Today, Type = TransactionType.Income, AccountId = _testAccount.Id },
            new() { Amount = 500m, Date = DateTime.Today, Type = TransactionType.Income, AccountId = _testAccount.Id },
            new() { Amount = 200m, Date = DateTime.Today, Type = TransactionType.Expense, AccountId = _testAccount.Id }
        };

        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.GetTotalIncomeAsync(_testAccount.Id, startDate, endDate);

        // Assert
        result.Should().Be(1500m);
    }

    [Fact]
    public async Task GetTotalExpensesAsync_ShouldReturnCorrectSum()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-30);
        var endDate = DateTime.Today;
        var transactions = new List<Transaction>
        {
            new() { Amount = 300m, Date = DateTime.Today, Type = TransactionType.Expense, AccountId = _testAccount.Id },
            new() { Amount = 150m, Date = DateTime.Today, Type = TransactionType.Expense, AccountId = _testAccount.Id },
            new() { Amount = 1000m, Date = DateTime.Today, Type = TransactionType.Income, AccountId = _testAccount.Id }
        };

        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.GetTotalExpensesAsync(_testAccount.Id, startDate, endDate);

        // Assert
        result.Should().Be(450m);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}