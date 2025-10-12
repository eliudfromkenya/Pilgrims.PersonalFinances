using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Core.Services;
using Pilgrims.PersonalFinances.Core.Interfaces;
using Pilgrims.PersonalFinances.Data;
using Xunit;

namespace Pilgrims.PersonalFinances.Tests.Services
{
    public class CurrencyServiceTests
    {
        private static PersonalFinanceContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<PersonalFinanceContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            return new PersonalFinanceContext(options);
        }

        [Fact]
        public async Task SetCurrentCurrencyAsync_ShouldPersistCurrency()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            ICurrencyService service = new CurrencyService(context);

            // Act
            await service.SetCurrentCurrencyAsync("EUR");
            var settings = await context.ApplicationSettings.FirstOrDefaultAsync(s => s.IsActive);

            // Assert
            settings.Should().NotBeNull();
            settings!.DefaultCurrency.Should().Be("EUR");
        }

        [Fact]
        public async Task GetCurrencySymbolAsync_ShouldReturnSymbol_ForCurrentCurrency()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var service = new CurrencyService(context);
            await service.SetCurrentCurrencyAsync("EUR");

            // Act
            var symbol = await service.GetCurrencySymbolAsync();

            // Assert
            symbol.Should().Be("€");
        }

        [Fact]
        public void GetCurrencySymbol_ShouldReturnSymbol_ForProvidedCode()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new CurrencyService(context);

            // Act
            var symbol = service.GetCurrencySymbol("JPY");

            // Assert
            symbol.Should().Be("¥");
        }

        [Fact]
        public async Task FormatAmountAsync_ShouldFormat_USD_Default()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var service = new CurrencyService(context);

            // Act
            var formatted = await service.FormatAmountAsync(1234.56m);

            // Assert
            formatted.Should().Contain("$");
            formatted.Should().Contain("1,234.56");
        }

        [Fact]
        public async Task FormatAmountAsync_ShouldIncludeCurrencyCode_WhenRequested()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var service = new CurrencyService(context);
            await service.SetCurrentCurrencyAsync("GBP");

            // Act
            var formatted = await service.FormatAmountAsync(100m, includeCurrencyCode: true);

            // Assert
            formatted.Should().Contain("£");
            formatted.Should().Contain("GBP");
        }

        [Fact]
        public void GetSupportedCurrencies_ShouldContainCommonCodes()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var service = new CurrencyService(context);

            // Act
            var currencies = service.GetSupportedCurrencies();

            // Assert
            currencies.Should().NotBeEmpty();
            currencies.Should().Contain(c => c.Code == "USD");
            currencies.Should().Contain(c => c.Code == "EUR");
        }
    }
}