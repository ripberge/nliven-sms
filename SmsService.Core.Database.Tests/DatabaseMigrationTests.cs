using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SmsService.Core.Data;
using Xunit;

namespace SmsService.Core.Database.Tests;

public class DatabaseMigrationTests
{
    /// <summary>
    /// This test ensures that when entity models change, a corresponding migration exists.
    /// It compares the model snapshot (from last migration) with the current DbContext model.
    /// If this test fails, run: dotnet ef migrations add <MigrationName> in SmsService.Migrations
    /// </summary>
    [Fact]
    public void EntityModel_ShouldMatchLatestMigration()
    {
        // Arrange - Configure DbContext with SQL Server provider (no actual connection needed for model comparison)
        var options = new DbContextOptionsBuilder<SmsServiceDbContext>()
            .UseSqlServer(
                "Server=dummy;Database=dummy;",
                b => b.MigrationsAssembly("SmsService.Migrations")
            )
            .Options;

        using var context = new SmsServiceDbContext(options);

        // Get the migrations assembly which contains the snapshot
        var migrationsAssembly = context.GetService<IMigrationsAssembly>();

        // Ensure we have migrations
        Assert.NotNull(migrationsAssembly.ModelSnapshot);
        Assert.NotEmpty(migrationsAssembly.Migrations);

        // Get both models for comparison
        var snapshotModel = migrationsAssembly.ModelSnapshot.Model;
        var currentModel = context.Model;

        // Simple check: compare entity counts
        var snapshotEntities = snapshotModel
            .GetEntityTypes()
            .Select(e => e.Name)
            .OrderBy(n => n)
            .ToList();
        var currentEntities = currentModel
            .GetEntityTypes()
            .Select(e => e.Name)
            .OrderBy(n => n)
            .ToList();

        Assert.Equal(snapshotEntities.Count, currentEntities.Count);

        // Compare entity names
        for (int i = 0; i < snapshotEntities.Count; i++)
        {
            Assert.Equal(snapshotEntities[i], currentEntities[i]);
        }

        // Compare property counts for each entity
        foreach (var entityName in currentEntities)
        {
            var snapshotEntity = snapshotModel.FindEntityType(entityName);
            var currentEntity = currentModel.FindEntityType(entityName);

            Assert.NotNull(snapshotEntity);
            Assert.NotNull(currentEntity);

            var snapshotProps = snapshotEntity
                .GetProperties()
                .Select(p => p.Name)
                .OrderBy(n => n)
                .ToList();
            var currentProps = currentEntity
                .GetProperties()
                .Select(p => p.Name)
                .OrderBy(n => n)
                .ToList();

            // Compare property names - provide detailed error on mismatch
            if (!snapshotProps.SequenceEqual(currentProps))
            {
                var missing = snapshotProps.Except(currentProps).ToList();
                var added = currentProps.Except(snapshotProps).ToList();
                var message = $"Property mismatch in entity '{entityName}'.\n";
                if (added.Any())
                {
                    message += $"  Added properties (need migration): {string.Join(", ", added)}\n";
                }
                if (missing.Any())
                {
                    message += $"  Removed properties (need migration): {string.Join(", ", missing)}\n";
                }
                Assert.Fail(message + "  Run: dotnet ef migrations add <MigrationName> in SmsService.Migrations");
            }

            Assert.Equal(snapshotProps.Count, currentProps.Count);
        }
    }
}
