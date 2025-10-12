using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Xunit;

namespace Pilgrims.PersonalFinances.Tests.Integration
{
    public class CurrencySeedMigrationIntegrationTests
    {
        private static string CreateUniqueDatabasePath()
        {
            var fileName = $"pf_seed_migrate_{Guid.NewGuid():N}.db";
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
        public async Task MigrateAsync_ShouldApplyMigrations_AndSeedCurrencies()
        {
            // Arrange
            var dbPath = CreateUniqueDatabasePath();
            try
            {
                await using var context = CreateContext(dbPath);

                // Act
                await context.Database.MigrateAsync();

                // Assert
                var count = await context.Currencies.CountAsync();
                var hasUsd = await context.Currencies.AnyAsync(c => c.ISOCode == "USD");
                var hasEur = await context.Currencies.AnyAsync(c => c.ISOCode == "EUR");

                count.Should().BeGreaterThan(0);
                hasUsd.Should().BeTrue();
                hasEur.Should().BeTrue();
            }
            finally
            {
                // Cleanup
                try
                {
                    await using var cleanupContext = CreateContext(dbPath);
                    await cleanupContext.Database.EnsureDeletedAsync();
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