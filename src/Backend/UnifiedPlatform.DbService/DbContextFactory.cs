using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SmallTarget.DbService.Entities;

namespace UnifiedPlatform.DbService;

/// <summary>
/// DbContext Factory for Entity Framework migrations
/// </summary>
public class DbContextFactory : IDesignTimeDbContextFactory<StDbContext>
{
    public StDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StDbContext>();
        
        // Default connection string for migrations
        // This can be overridden by environment variables or appsettings.json
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=UnifiedWeb3Platform;Integrated Security=True;TrustServerCertificate=True;";
        
        optionsBuilder.UseSqlServer(connectionString);
        
        return new StDbContext(optionsBuilder.Options);
    }
}

