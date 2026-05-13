using AtomicTicket.Infrastructure.Persistence.Write;
using AtomicTicket.SharedKernel.Domain;
using DnsClient.Internal;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;

namespace AtomicTicket.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class OutboxProcessor(
    ApplicationDbContext dbContext,
    IPublisher publisher,
    ILogger logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await dbContext.Set<OutboxMessage>()
        .Where(m => m.ProcessedOnUtc == null)
        .Take(20)
        .ToListAsync(context.CancellationToken);

        foreach (var message in messages)
        {
            try
            {
                if (message.EventKind == EventKind.Domain)
                {
                    var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(message.Content);

                    if (domainEvent == null)
                    {
                        logger.LogError("Failed to deserialize outbox message with ID: {MessageId}. Content: {Content}", message.Id, message.Content);
                        message.Error = "Deserialization returned null";
                    }
                    else
                    {
                        await publisher.Publish(domainEvent, context.CancellationToken);
                    }
                }

                else if (message.EventKind == EventKind.Integration)
                {
                    var integrationEvent = JsonConvert.DeserializeObject<IIntegrationEvent>(message.Content);

                    if (integrationEvent == null)
                    {
                        logger.LogError("Failed to deserialize outbox message with ID: {MessageId}. Content: {Content}", message.Id, message.Content);
                        message.Error = "Deserialization returned null";
                    }
                    else
                    {
                        //await publisher.Publish(domainEvent, context.CancellationToken);
                    }
                }

                message.ProcessedOnUtc = DateTime.UtcNow;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred while processing outbox message {MessageId}", message.Id);
                message.Error = ex.Message;
            }

            await dbContext.SaveChangesAsync(context.CancellationToken);
        }
    }
}
