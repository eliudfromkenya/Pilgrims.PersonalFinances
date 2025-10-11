using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Models.Enums;
using Pilgrims.PersonalFinances.Services;
using Pilgrims.PersonalFinances.Tests.Data;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pilgrims.PersonalFinances.Core.Interfaces;

namespace Pilgrims.PersonalFinances.Tests.Services;

public class AccountServiceTests : IDisposable
{
    private readonly TestPersonalFinanceContext _context;
    private readonly AccountService _accountService;
    private readonly ServiceProvider _serviceProvider;

    public AccountServiceTests()
    {
        // Create a service collection for Entity Framework services
        var services = new ServiceCollection();
        services.AddEntityFrameworkInMemoryDatabase();
        _serviceProvider = services.BuildServiceProvider();
        
        // Create DbContext options with the service provider
        var options = new DbContextOptionsBuilder<PersonalFinanceContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .UseInternalServiceProvider(_serviceProvider)
            .EnableSensitiveDataLogging()
            .Options;
        
        _context = new TestPersonalFinanceContext(options);
        var mockCurrencyService = new Mock<ICurrencyService>();
        mockCurrencyService.Setup(x => x.FormatAmount(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<bool>())).Returns("$0.00");
        mockCurrencyService.Setup(x => x.GetCurrentCurrencyAsync()).ReturnsAsync("USD");
        _accountService = new AccountService(_context, mockCurrencyService.Object);
    }

    [Fact]
    public async Task CreateAccountAsync_ShouldCreateAccount_WhenValidDataProvided()
    {
        // Arrange
        var account = new Account
        {
            Name = "Test Checking Account",
            AccountType = AccountType.Checking,
            InitialBalance = 1000.00m,
            CurrentBalance = 1000.00m,
            Currency = "USD"
        };

        // Act
        var result = await _accountService.CreateAccountAsync(account);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Test Checking Account");
        result.AccountType.Should().Be(AccountType.Checking);
        result.CurrentBalance.Should().Be(1000.00m);
    }

    [Fact]
    public async Task GetAccountByIdAsync_ShouldReturnAccount_WhenAccountExists()
    {
        // Arrange
        var account = new Account
        {
            Name = "Test Savings Account",
            AccountType = AccountType.Checking,
            InitialBalance = 5000.00m,
            CurrentBalance = 5000.00m,
            Currency = "USD"
        };
        
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await _accountService.GetAccountByIdAsync(account.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(account.Id);
        result.Name.Should().Be("Test Savings Account");
    }

    [Fact]
    public async Task GetAccountByIdAsync_ShouldReturnNull_WhenAccountDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid().ToString();

        // Act
        var result = await _accountService.GetAccountByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAccountsAsync_ShouldReturnAllAccounts()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new() { Name = "Account 1", AccountType = AccountType.Checking, InitialBalance = 1000m, CurrentBalance = 1000m, Currency = "USD" },
            new() { Name = "Account 2", AccountType = AccountType.Savings, InitialBalance = 2000m, CurrentBalance = 2000m, Currency = "USD" },
            new() { Name = "Account 3", AccountType = AccountType.Credit, InitialBalance = -500m, CurrentBalance = -500m, Currency = "USD" }
        };

        await _context.Accounts.AddRangeAsync(accounts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _accountService.GetAllAccountsAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(a => a.Name == "Account 1");
        result.Should().Contain(a => a.Name == "Account 2");
        result.Should().Contain(a => a.Name == "Account 3");
    }

    [Fact]
    public async Task UpdateAccountAsync_ShouldUpdateAccount_WhenValidDataProvided()
    {
        // Arrange
        var account = new Account
        {
            Name = "Original Name",
            AccountType = AccountType.Checking,
            InitialBalance = 1000m,
            CurrentBalance = 1000m,
            Currency = "USD"
        };
        
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        account.Name = "Updated Name";
        account.CurrentBalance = 1500m;

        // Act
        var result = await _accountService.UpdateAccountAsync(account);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
        result.CurrentBalance.Should().Be(1500m);
    }

    [Fact]
    public async Task DeleteAccountAsync_ShouldDeleteAccount_WhenAccountExists()
    {
        // Arrange
        var account = new Account
        {
            Name = "Test Account",
            AccountType = AccountType.Checking,
            InitialBalance = 1000m,
            CurrentBalance = 1000m,
            Currency = "USD"
        };
        
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await _accountService.DeleteAccountAsync(account.Id);

        // Assert
        result.Should().BeTrue();
        
        var deletedAccount = await _context.Accounts.FindAsync(account.Id);
        deletedAccount.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAccountAsync_ShouldReturnFalse_WhenAccountDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid().ToString();

        // Act
        var result = await _accountService.DeleteAccountAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(AccountType.Checking)]
    [InlineData(AccountType.Savings)]
    [InlineData(AccountType.Credit)]
    [InlineData(AccountType.Investment)]
    public async Task GetAccountsByTypeAsync_ShouldReturnAccountsOfSpecificType(AccountType accountType)
    {
        // Arrange
        var accounts = new List<Account>
        {
            new() { Name = "Checking 1", AccountType = AccountType.Checking, InitialBalance = 1000m, CurrentBalance = 1000m, Currency = "USD" },
            new() { Name = "Savings 1", AccountType = AccountType.Savings, InitialBalance = 2000m, CurrentBalance = 2000m, Currency = "USD" },
            new() { Name = "Credit 1", AccountType = AccountType.Credit, InitialBalance = -500m, CurrentBalance = -500m, Currency = "USD" },
            new() { Name = "Investment 1", AccountType = AccountType.Investment, InitialBalance = 5000m, CurrentBalance = 5000m, Currency = "USD" }
        };

        await _context.Accounts.AddRangeAsync(accounts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _accountService.GetAccountsByTypeAsync(accountType);

        // Assert
        result.Should().HaveCount(1);
        result.First().AccountType.Should().Be(accountType);
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}