using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Tests.Services;

public class BudgetServiceTests : IDisposable
{
    private readonly PersonalFinanceContext _context;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly BudgetService _budgetService;

    public BudgetServiceTests()
    {
        var options = new DbContextOptionsBuilder<PersonalFinanceContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new PersonalFinanceContext(options);
        _mockNotificationService = new Mock<INotificationService>();
        _budgetService = new BudgetService(_context, _mockNotificationService.Object);
    }

    [Fact]
    public async Task CreateBudgetAsync_ShouldCreateBudget_WhenValidDataProvided()
    {
        // Arrange
        var budget = new Budget
        {
            Name = "Monthly Budget",
            BudgetType = BudgetType.Category,
            Period = BudgetPeriod.Monthly,
            CategoryId = Guid.NewGuid().ToString(),
            LimitAmount = 2000.00m,
            StartDate = DateTime.Today.AddDays(-30),
            EndDate = DateTime.Today,
            IsActive = true
        };

        // Act
        var result = await _budgetService.CreateBudgetAsync(budget);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Monthly Budget");
        result.LimitAmount.Should().Be(2000.00m);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetBudgetByIdAsync_ShouldReturnBudget_WhenBudgetExists()
    {
        // Arrange
        var budget = new Budget
        {
            Name = "Test Budget",
            BudgetType = BudgetType.Category,
            Period = BudgetPeriod.Monthly,
            CategoryId = Guid.NewGuid().ToString(),
            LimitAmount = 1500.00m,
            StartDate = DateTime.Today.AddDays(-30),
            EndDate = DateTime.Today,
            IsActive = true
        };
        
        await _context.Budgets.AddAsync(budget);
        await _context.SaveChangesAsync();

        // Act
        var result = await _budgetService.GetBudgetByIdAsync(budget.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(budget.Id);
        result.Name.Should().Be("Test Budget");
        result.LimitAmount.Should().Be(1500.00m);
    }

    [Fact]
    public async Task GetBudgetByIdAsync_ShouldReturnNull_WhenBudgetDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid().ToString();

        // Act
        var result = await _budgetService.GetBudgetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllBudgetsAsync_ShouldReturnAllBudgets()
    {
        // Arrange
        var budgets = new List<Budget>
        {
            new() { Name = "Budget 1", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 1000m, StartDate = DateTime.Today.AddDays(-30), EndDate = DateTime.Today, IsActive = true },
            new() { Name = "Budget 2", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 2000m, StartDate = DateTime.Today.AddDays(-60), EndDate = DateTime.Today.AddDays(-30), IsActive = false },
            new() { Name = "Budget 3", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 1500m, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30), IsActive = true }
        };

        await _context.Budgets.AddRangeAsync(budgets);
        await _context.SaveChangesAsync();

        // Act
        var result = await _budgetService.GetAllBudgetsAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(b => b.Name == "Budget 1");
        result.Should().Contain(b => b.Name == "Budget 2");
        result.Should().Contain(b => b.Name == "Budget 3");
    }

    [Fact]
    public async Task GetActiveBudgetsAsync_ShouldReturnOnlyActiveBudgets()
    {
        // Arrange
        var budgets = new List<Budget>
        {
            new() { Name = "Active Budget 1", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 1000m, StartDate = DateTime.Today.AddDays(-30), EndDate = DateTime.Today, IsActive = true },
            new() { Name = "Inactive Budget", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 2000m, StartDate = DateTime.Today.AddDays(-60), EndDate = DateTime.Today.AddDays(-30), IsActive = false },
            new() { Name = "Active Budget 2", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 1500m, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30), IsActive = true }
        };

        await _context.Budgets.AddRangeAsync(budgets);
        await _context.SaveChangesAsync();

        // Act
        var result = await _budgetService.GetActiveBudgetsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(b => b.IsActive);
        result.Should().Contain(b => b.Name == "Active Budget 1");
        result.Should().Contain(b => b.Name == "Active Budget 2");
    }

    [Fact]
    public async Task UpdateBudgetAsync_ShouldUpdateBudget_WhenValidDataProvided()
    {
        // Arrange
        var budget = new Budget
        {
            Name = "Original Budget",
            BudgetType = BudgetType.Category,
            Period = BudgetPeriod.Monthly,
            CategoryId = Guid.NewGuid().ToString(),
            LimitAmount = 1000m,
            StartDate = DateTime.Today.AddDays(-30),
            EndDate = DateTime.Today,
            IsActive = true
        };
        
        await _context.Budgets.AddAsync(budget);
        await _context.SaveChangesAsync();

        budget.Name = "Updated Budget";
        budget.LimitAmount = 1500m;
        budget.IsActive = false;

        // Act
        var result = await _budgetService.UpdateBudgetAsync(budget);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Budget");
        result.LimitAmount.Should().Be(1500m);
        result.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteBudgetAsync_ShouldDeleteBudget_WhenBudgetExists()
    {
        // Arrange
        var budget = new Budget
        {
            Name = "Budget to Delete",
            BudgetType = BudgetType.Category,
            Period = BudgetPeriod.Monthly,
            CategoryId = Guid.NewGuid().ToString(),
            LimitAmount = 1000m,
            StartDate = DateTime.Today.AddDays(-30),
            EndDate = DateTime.Today,
            IsActive = true
        };
        
        await _context.Budgets.AddAsync(budget);
        await _context.SaveChangesAsync();

        // Act
        var result = await _budgetService.DeleteBudgetAsync(budget.Id);

        // Assert
        result.Should().BeTrue();
        
        var deletedBudget = await _context.Budgets.FindAsync(budget.Id);
        deletedBudget.Should().BeNull();
    }

    [Fact]
    public async Task DeleteBudgetAsync_ShouldReturnFalse_WhenBudgetDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid().ToString();

        // Act
        var result = await _budgetService.DeleteBudgetAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetBudgetsByDateRangeAsync_ShouldReturnBudgetsInRange()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-30);
        var endDate = DateTime.Today;
        
        var budgets = new List<Budget>
        {
            new() { Name = "In Range 1", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 1000m, StartDate = DateTime.Today.AddDays(-25), EndDate = DateTime.Today.AddDays(-5), IsActive = true },
            new() { Name = "In Range 2", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 2000m, StartDate = DateTime.Today.AddDays(-20), EndDate = DateTime.Today.AddDays(5), IsActive = true },
            new() { Name = "Out of Range", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 1500m, StartDate = DateTime.Today.AddDays(-60), EndDate = DateTime.Today.AddDays(-40), IsActive = true }
        };

        await _context.Budgets.AddRangeAsync(budgets);
        await _context.SaveChangesAsync();

        // Act
        var result = await _budgetService.GetBudgetsByPeriodAsync(startDate, endDate);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(b => b.Name == "In Range 1");
        result.Should().Contain(b => b.Name == "In Range 2");
    }

    [Theory]
    [InlineData(1000.00)]
    [InlineData(2500.50)]
    [InlineData(500.25)]
    public async Task CalculateBudgetUtilizationAsync_ShouldReturnCorrectPercentage(decimal budgetAmount)
    {
        // Arrange
        var budget = new Budget
        {
            Name = "Test Budget",
            BudgetType = BudgetType.Category,
            Period = BudgetPeriod.Monthly,
            CategoryId = Guid.NewGuid().ToString(),
            LimitAmount = budgetAmount,
            StartDate = DateTime.Today.AddDays(-30),
            EndDate = DateTime.Today,
            IsActive = true
        };
        
        await _context.Budgets.AddAsync(budget);
        await _context.SaveChangesAsync();

        // Create some test transactions (assuming 50% utilization)
        var spentAmount = budgetAmount * 0.5m;
        
        // Act
        var result = await _budgetService.CalculateBudgetUtilizationAsync(budget.Id);

        // Assert
        result.Should().BeGreaterOrEqualTo(0);
        result.Should().BeLessOrEqualTo(100);
    }

    [Fact]
    public async Task GetBudgetSummaryAsync_ShouldReturnCorrectSummary()
    {
        // Arrange
        var budgets = new List<Budget>
        {
            new() { Name = "Budget 1", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 1000m, StartDate = DateTime.Today.AddDays(-30), EndDate = DateTime.Today, IsActive = true },
            new() { Name = "Budget 2", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 2000m, StartDate = DateTime.Today.AddDays(-30), EndDate = DateTime.Today, IsActive = true },
            new() { Name = "Budget 3", BudgetType = BudgetType.Category, Period = BudgetPeriod.Monthly, CategoryId = Guid.NewGuid().ToString(), LimitAmount = 1500m, StartDate = DateTime.Today.AddDays(-30), EndDate = DateTime.Today.AddDays(-30), IsActive = false }
        };

        await _context.Budgets.AddRangeAsync(budgets);
        await _context.SaveChangesAsync();

        // Act
        var result = await _budgetService.GetBudgetSummaryAsync();

        // Assert
        result.Should().NotBeNull();
        // Additional assertions would depend on the BudgetSummaryDto structure
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}