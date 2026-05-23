using AtomicTicket.Application.Abstractions;
using AtomicTicket.SharedKernel.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AtomicTicket.Infrastructure.Outbox;

internal sealed class IntegrationEventOutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<IntegrationEventOutboxProcessor> logger) : OutboxProcessorBase(scopeFactory, logger)
{
    protected override Expression<Func<OutboxMessage, bool>> GetMessageFilter() => m => m.EventKind == EventKind.Integration;

    protected override async Task ProcessAndPublishEventAsync(OutboxMessage message, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var eventbus = serviceProvider.GetRequiredService<IEventBus>();
        var integrationEvent = DeserializeContent(message.Content) as IIntegrationEvent;

        if (integrationEvent is not null)
        {
            await eventbus.PublishAsync(integrationEvent, cancellationToken);
        }
    }
}
