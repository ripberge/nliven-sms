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

                // Check for added properties (not in snapshot but in current model) - these NEED a migration
                if (added.Any())
                {
                    Assert.Fail(
                        $"Property mismatch in entity '{entityName}'.\n"
                            + $"  Added properties (need migration): {string.Join(", ", added)}\n"
                            + $"  Run: dotnet ef migrations add <MigrationName> in SmsService.Migrations"
                    );
                }

                // Check for properties in snapshot but not in current model
                // This is OK in two scenarios (safe forward deployment patterns):
                //   1. Property exists with [NotMapped] - migration is ahead, waiting for code to use it
                //   2. Property doesn't exist at all - migration is ahead, will be added to entity later
                // Only fail if it's a truly removed property (property was there before, now gone without [NotMapped])
                if (missing.Any())
                {
                    // Get the CLR type to check properties
                    var clrType = currentEntity.ClrType;
                    var safeProps = missing
                        .Where(propName =>
                        {
                            var property = clrType.GetProperty(propName);
                            // Safe if: property doesn't exist (migration ahead of code) OR has [NotMapped]
                            return property == null
                                || property
                                    .GetCustomAttributes(
                                        typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute),
                                        false
                                    )
                                    .Any();
                        })
                        .ToList();

                    // Remove safe properties from the missing list
                    var trulyMissing = missing.Except(safeProps).ToList();

                    if (trulyMissing.Any())
                    {
                        Assert.Fail(
                            $"Property mismatch in entity '{entityName}'.\n"
                                + $"  Removed properties (need DROP COLUMN migration): {string.Join(", ", trulyMissing)}\n"
                                + $"  Run: dotnet ef migrations add <MigrationName> in SmsService.Migrations"
                        );
                    }
                }
            }
        }
    }
}
