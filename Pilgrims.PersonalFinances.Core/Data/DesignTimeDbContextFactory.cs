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
        
#if ANDROID
        optionsBuilder.UseInMemoryDatabase("PersonalFinance_Migration");
#else
        optionsBuilder.UseSqlite(connectionString);
#endif
        
        return new PersonalFinanceContext(optionsBuilder.Options);
    }
}