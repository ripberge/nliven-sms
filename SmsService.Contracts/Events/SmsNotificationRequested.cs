namespace SmsService.Contracts.Events;

/// <summary>
/// Event published when an SMS notification is requested (e.g., order confirmation)
/// </summary>
public class SmsNotificationRequested
{
    public required int VenueId { get; set; }

    /// <summary>
    /// Receiver phone number - must include country code (e.g., 15555555555)
    /// </summary>
    public required string ReceiverPhone { get; set; }

    public required string Message { get; set; }
}
