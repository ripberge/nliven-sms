using SmsService.Contracts.Events;

namespace SmsService.Infrastructure.Interfaces;

public interface ISmsNotificationRequestedHandler
{
    /// <summary>
    /// Handles the incoming SMS notification request message.
    /// Throws an exception if processing fails.
    /// </summary>
    Task HandleMessage(
        SmsNotificationRequested notification,
        CancellationToken cancellationToken = default
    );
}
