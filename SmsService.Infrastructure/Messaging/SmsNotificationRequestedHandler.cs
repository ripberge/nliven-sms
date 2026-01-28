using Microsoft.Extensions.Logging;
using SmsService.Contracts.Events;
using SmsService.Infrastructure.Interfaces;

namespace SmsService.Infrastructure.Messaging;

public class SmsNotificationRequestedHandler(ILogger<SmsNotificationRequestedHandler> logger)
    : ISmsNotificationRequestedHandler
{
    public Task HandleMessage(
        SmsNotificationRequested notification,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation(
            "Starting to handle SMS for VenueId: {VenueId}",
            notification.VenueId
        );

        // temp! We need to do *something* for real here!
        return Task.CompletedTask;
    }
}
