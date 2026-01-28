using SmsService.Domain.Enums;

namespace SmsService.Domain.Entities;

public class VenuePhoneNumber
{
    public int Id { get; set; }
    public int VenueId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>External Id, comes from the third-party provider</summary>
    public string ProviderId { get; set; } = string.Empty;
    public int SmsProviderId { get; set; }
    public VenuePhoneNumberStatus Status { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public SmsProvider SmsProvider { get; set; } = null!;
}
