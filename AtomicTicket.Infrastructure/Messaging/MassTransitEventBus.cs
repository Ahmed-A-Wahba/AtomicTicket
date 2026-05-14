using AtomicTicket.Application.Abstractions;
using MassTransit;

namespace AtomicTicket.Infrastructure.Messaging;

internal sealed class MassTransitEventBus(IPublishEndpoint bus) : IEventBus
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        await bus.Publish(message, message.GetType(), cancellationToken);
    }
}
