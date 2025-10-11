using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pilgrims.PersonalFinances.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PersonalFinanceContext>
{
    public PersonalFinanceContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PersonalFinanceContext>();
        
        // Use a temporary database path for migrations
        var tempDbPath = Path.Combine(Path.GetTempPath(), "PersonalFinance_Migration.db");
        var connectionString = $"Data Source={tempDbPath};";
        
        optionsBuilder.UseSqlite(connectionString);
        
        return new PersonalFinanceContext(optionsBuilder.Options);
    }
}