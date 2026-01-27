using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SmsService.Core.Data;

namespace SmsService.Migrations.Data;

public class SmsServiceDbContextFactory : IDesignTimeDbContextFactory<SmsServiceDbContext>
{
    public SmsServiceDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SmsServiceDbContext>();

        // Default connection string for migrations - can be overridden via appsettings.json
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Database=SmsServiceDb;Trusted_Connection=True;TrustServerCertificate=True;";

        optionsBuilder.UseSqlServer(
            connectionString,
            b => b.MigrationsAssembly("SmsService.Migrations")
        );

        return new SmsServiceDbContext(optionsBuilder.Options);
    }
}
