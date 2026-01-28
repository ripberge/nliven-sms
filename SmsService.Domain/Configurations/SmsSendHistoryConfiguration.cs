using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmsService.Domain.Entities;

namespace SmsService.Domain.Configurations;

public class SmsSendHistoryConfiguration : IEntityTypeConfiguration<SmsSendHistory>
{
    public void Configure(EntityTypeBuilder<SmsSendHistory> builder)
    {
        builder.ToTable("SmsSendHistory");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.OrderId).IsRequired();
        builder.HasIndex(s => s.OrderId);

        builder.Property(s => s.VenueId).IsRequired();
        builder.HasIndex(s => s.VenueId);

        builder.Property(s => s.VenuePhoneNumberId).IsRequired();

        builder.Property(s => s.CustomerId).IsRequired();

        builder.Property(s => s.CustomerPhoneNumber).IsRequired().HasMaxLength(20);
        builder.HasIndex(s => s.CustomerPhoneNumber);

        builder.Property(s => s.Message).IsRequired().HasMaxLength(500);

        builder.Property(s => s.SmsProviderId).IsRequired();

        builder.Property(s => s.Status).IsRequired();

        builder.Property(s => s.ProviderMessageId).HasMaxLength(100);

        builder.Property(s => s.ErrorCode).HasMaxLength(50);

        builder.Property(s => s.CreatedAt).IsRequired().HasColumnType("datetime2");
        builder.HasIndex(s => s.CreatedAt);

        // Composite indexes
        builder.HasIndex(s => new { s.VenueId, s.CreatedAt });
        builder.HasIndex(s => new { s.CustomerPhoneNumber, s.VenueId });

        // Foreign key
        builder
            .HasOne(s => s.SmsProvider)
            .WithMany()
            .HasForeignKey(s => s.SmsProviderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
