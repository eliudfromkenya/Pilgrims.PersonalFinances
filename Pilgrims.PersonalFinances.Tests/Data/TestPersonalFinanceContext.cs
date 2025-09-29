using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;

namespace Pilgrims.PersonalFinances.Tests.Data;

/// <summary>
/// Test-specific PersonalFinanceContext that doesn't depend on MAUI APIs
/// </summary>
public class TestPersonalFinanceContext : PersonalFinanceContext
{
    public TestPersonalFinanceContext(DbContextOptions<PersonalFinanceContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Don't call base.OnConfiguring to avoid MAUI FileSystem dependency
        // The options are already configured through the constructor
        
        // Suppress the pending model changes warning for tests
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }
}