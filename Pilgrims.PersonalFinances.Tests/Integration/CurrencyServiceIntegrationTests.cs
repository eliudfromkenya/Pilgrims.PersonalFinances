using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Core.Services;
using Pilgrims.PersonalFinances.Core.Interfaces;
using Pilgrims.PersonalFinances.Data;
using Xunit;

namespace Pilgrims.PersonalFinances.Tests.Integration
{
    public class CurrencyServiceIntegrationTests
    {
        private static string CreateUniqueDatabasePath()
        {
            var fileName = $"pf_currency_{Guid.NewGuid():N}.db";
            return Path.Combine(Path.GetTempPath(), fileName);
        }

        private static PersonalFinanceContext CreateContext(string dbPath)
        {
            var options = new DbContextOptionsBuilder<PersonalFinanceContext>()
                .UseSqlite($"Data Source={dbPath}", b => b.MigrationsAssembly("Pilgrims.PersonalFinances.Core"))
                .EnableSensitiveDataLogging()
                .Options;

            return new PersonalFinanceContext(options);
        }

        [Fact]
        public async Task GetCurrentCurrencyAsync_ShouldCreateDefaultSettings_WhenNoneExist()
        {
            // Arrange
            var dbPath = CreateUniqueDatabasePath();
            try
            {
                await using var context = CreateContext(dbPath);
                await context.Database.EnsureCreatedAsync();

                ICurrencyService currencyService = new CurrencyService(context);

                // Act
                var defaultCurrency = await currencyService.GetCurrentCurrencyAsync();

                // Assert
                defaultCurrency.Should().Be("USD");

                // And settings should exist in DB
                var settingsCount = await context.ApplicationSettings.CountAsync();
                settingsCount.Should().BeGreaterThan(0);

                await context.Database.EnsureDeletedAsync();
            }
            finally
            {
                // Cleanup
                try
                {
                    await using var contextCleanup = CreateContext(dbPath);
                    await contextCleanup.Database.EnsureDeletedAsync();
                }
                catch
                {
                    // ignore
                }

                try
                {
                    if (File.Exists(dbPath))
                    {
                        File.Delete(dbPath);
                    }
                }
                catch (IOException)
                {
                    // ignore file in use
                }
                catch (UnauthorizedAccessException)
                {
                    // ignore permission
                }
            }
        }
    }
}