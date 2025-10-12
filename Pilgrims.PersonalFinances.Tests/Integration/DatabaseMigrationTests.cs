using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Xunit;

namespace Pilgrims.PersonalFinances.Tests.Integration
{
    public class DatabaseMigrationTests
    {
        private static string CreateUniqueDatabasePath()
        {
            var fileName = $"pf_integration_{Guid.NewGuid():N}.db";
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
        public async Task Migrate_ShouldCreateCoreTables_OnSqliteDatabase()
        {
            // Arrange
            var dbPath = CreateUniqueDatabasePath();
            try
            {
                await using var context = CreateContext(dbPath);

                // Act
                await context.Database.EnsureCreatedAsync();

                // Assert
                await using var connection = context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                var expectedTables = new[]
                {
                    "Accounts",
                    "ApplicationSettings",
                    "Currencies",
                    "Transactions"
                };

                var existingTables = new List<string>();
                await using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                    await using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        existingTables.Add(reader.GetString(0));
                    }
                }

                expectedTables.All(t => existingTables.Contains(t)).Should().BeTrue(
                    $"Database should contain tables: {string.Join(", ", expectedTables)} but had: {string.Join(", ", existingTables)}");

                // Close connection explicitly before cleanup
                await connection.CloseAsync();
            }
            finally
            {
                // Ensure database is deleted to release file locks before cleanup
                try
                {
                    await using var contextCleanup = CreateContext(dbPath);
                    await contextCleanup.Database.EnsureDeletedAsync();
                }
                catch
                {
                    // Ignore errors during EF Core database deletion
                }

                // Cleanup fallback: attempt to delete the file if it still exists
                try
                {
                    if (File.Exists(dbPath))
                    {
                        File.Delete(dbPath);
                    }
                }
                catch (IOException)
                {
                    // Ignore file in use errors in test cleanup
                }
                catch (UnauthorizedAccessException)
                {
                    // Ignore permission issues in test cleanup
                }
            }
        }
    }
}