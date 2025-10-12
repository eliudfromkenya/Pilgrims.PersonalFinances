using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Core.Data;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.Enums;
using Pilgrims.PersonalFinances.Core.Services;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using Xunit;

namespace Pilgrims.PersonalFinances.Tests.Services
{
    public class AccountTypeServiceTests
    {
        private static PersonalFinanceContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<PersonalFinanceContext>()
                .UseInMemoryDatabase(databaseName: $"pf-{Guid.NewGuid()}")
                .Options;
            return new PersonalFinanceContext(options);
        }

        [Fact]
        public async Task GetActiveTypesAsync_ReturnsOnlyActiveOrderedByName()
        {
            // Arrange
            using var ctx = CreateContext();
            ctx.Set<AccountTypeDefinition>().AddRange(
                new AccountTypeDefinition { Name = "Savings", Description = "Emergency fund", Icon = "💰", EnumValue = (int)AccountType.Savings, IsActive = true },
                new AccountTypeDefinition { Name = "Checking", Description = "Daily spending", Icon = "🏦", EnumValue = (int)AccountType.Checking, IsActive = true },
                new AccountTypeDefinition { Name = "ClosedType", Description = "Not used", Icon = "❌", EnumValue = 999, IsActive = false }
            );
            await ctx.SaveChangesAsync();
            IAccountTypeService service = new AccountTypeService(ctx);

            // Act
            var results = await service.GetActiveTypesAsync(CancellationToken.None);

            // Assert
            results.Should().HaveCount(2);
            results.Select(x => x.Name).Should().ContainInOrder("Checking", "Savings");
        }

        [Fact]
        public async Task GetByEnumAsync_ReturnsMatchingDefinition()
        {
            using var ctx = CreateContext();
            ctx.Set<AccountTypeDefinition>().Add(new AccountTypeDefinition { Name = "Savings", EnumValue = (int)AccountType.Savings, Icon = "💰", Description = "Emergency fund" });
            await ctx.SaveChangesAsync();
            var service = new AccountTypeService(ctx);

            var def = await service.GetByEnumAsync(AccountType.Savings);
            def.Should().NotBeNull();
            def!.Name.Should().Be("Savings");
        }

        [Fact]
        public async Task GetIconByEnumAsync_ReturnsIconOrEmpty()
        {
            using var ctx = CreateContext();
            var service = new AccountTypeService(ctx);
            var iconEmpty = await service.GetIconByEnumAsync(AccountType.Checking);
            iconEmpty.Should().BeEmpty();

            ctx.Set<AccountTypeDefinition>().Add(new AccountTypeDefinition { EnumValue = (int)AccountType.Checking, Icon = "🏦" });
            await ctx.SaveChangesAsync();
            var icon = await service.GetIconByEnumAsync(AccountType.Checking);
            icon.Should().Be("🏦");
        }
    }
}