using Microsoft.EntityFrameworkCore;
using SmsService.Core.Entities;

namespace SmsService.Core.Data;

public class SmsServiceDbContext : DbContext
{
    public SmsServiceDbContext(DbContextOptions<SmsServiceDbContext> options) 
        : base(options)
    {
    }

    public DbSet<ProviderName> ProviderNames => Set<ProviderName>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProviderName>(entity =>
        {
            entity.ToTable("ProviderNames");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.HasIndex(e => e.Name)
                .IsUnique();
            
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");
        });
    }
}
