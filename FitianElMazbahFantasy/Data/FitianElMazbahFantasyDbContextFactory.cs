using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FitianElMazbahFantasy.Data;

public class FitianElMazbahFantasyDbContextFactory : IDesignTimeDbContextFactory<FitianElMazbahFantasyDbContext>
{
    public FitianElMazbahFantasyDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<FitianElMazbahFantasyDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        optionsBuilder.UseSqlServer(connectionString);

        return new FitianElMazbahFantasyDbContext(optionsBuilder.Options);
    }
}