using AtomicTicket.SharedKernel.Domain;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AtomicTicket.Infrastructure.Outbox;

internal sealed class DomainEventOutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<DomainEventOutboxProcessor> logger) : OutboxProcessorBase(scopeFactory, logger)
{
    protected override Expression<Func<OutboxMessage, bool>> GetMessageFilter() => m => m.EventKind == EventKind.Domain;

    protected override async Task ProcessAndPublishEventAsync(OutboxMessage message, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var publisher = serviceProvider.GetRequiredService<IPublisher>();
        var domainEvent = DeserializeContent(message.Content) as IDomainEvent;

        if(domainEvent is not null)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
