using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmsService.Contracts.Events;
using SmsService.Infrastructure.Interfaces;

namespace SmsService.Infrastructure.Messaging;

public class ServiceBusConsumer
{
    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<ServiceBusConsumer> _logger;
    private readonly ISmsNotificationRequestedHandler _messageHandler;

    public ServiceBusConsumer(
        string connectionString,
        string topicName,
        string subscriptionName,
        ISmsNotificationRequestedHandler messageHandler,
        ILogger<ServiceBusConsumer> logger
    )
    {
        _logger = logger;
        _messageHandler = messageHandler;

        var client = new ServiceBusClient(connectionString);
        _processor = client.CreateProcessor(
            topicName,
            subscriptionName,
            new ServiceBusProcessorOptions { AutoCompleteMessages = false, MaxConcurrentCalls = 1 }
        );

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Service Bus consumer");
        await _processor.StartProcessingAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping Service Bus consumer");
        await _processor.StopProcessingAsync(cancellationToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            _logger.LogDebug("Starting to process message. {MessageId}", args.Message.MessageId);

            var messageBody = args.Message.Body.ToString();
            var notification = JsonConvert.DeserializeObject<SmsNotificationRequested>(messageBody);

            if (notification == null)
            {
                _logger.LogError(
                    "Message body deserialized to null: {MessageId}",
                    args.Message.MessageId
                );
                await args.DeadLetterMessageAsync(
                    args.Message,
                    "NullMessage",
                    "Message body deserialized to null"
                );
                return;
            }

            await _messageHandler.HandleMessage(notification, args.CancellationToken);

            await args.CompleteMessageAsync(args.Message);
            _logger.LogInformation(
                "Successfully processed message. Message {MessageId}",
                args.Message.MessageId
            );
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Failed to deserialize message into SmsNotificationRequested. MessageId: {MessageId}. Dead-lettering.",
                args.Message.MessageId
            );
            await args.DeadLetterMessageAsync(
                args.Message,
                "DeserializationFailed",
                "Invalid JSON format"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing message: {MessageId}. Will retry.",
                args.Message.MessageId
            );

            // Abandon the message to retry
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(
            args.Exception,
            "Error in Service Bus processor. Source: {ErrorSource}, EntityPath: {EntityPath}",
            args.ErrorSource,
            args.EntityPath
        );
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _processor.DisposeAsync();
    }
}
