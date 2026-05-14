using AtomicTicket.Application.Abstractions;
using AtomicTicket.Infrastructure.Outbox;
using AtomicTicket.Infrastructure.Persistence.Write;
using AtomicTicket.SharedKernel.Domain;
using AtomicTicket.SharedKernel.Primitives;
using MassTransit.Transports;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

[DisallowConcurrentExecution]
internal sealed class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessor> logger) : IJob
{
    private static readonly JsonSerializerSettings _settings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Processing outbox messages...");
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
        var eventBus = scope.ServiceProvider.GetService<IEventBus>();

        var messages = await dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null && m.Error == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        if (messages.Count == 0) return;

        foreach (var message in messages)
        {
            try
            {
                if (message.EventKind == EventKind.Domain)
                {
                    var domainEvent = JsonConvert.DeserializeObject<object>(message.Content, _settings) as IDomainEvent;

                    if (domainEvent is not null)
                    {
                        await publisher.Publish(domainEvent, context.CancellationToken);
                    }
                }
                else if (message.EventKind == EventKind.Integration)
                {
                    var integrationEvent = JsonConvert.DeserializeObject<object>(message.Content, _settings) as IIntegrationEvent;

                    if (integrationEvent is not null)
                    {
                        await eventBus!.PublishAsync(integrationEvent,  context.CancellationToken);
                    }
                }

                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                message.Error = ex.Message;
            }
        }
        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}