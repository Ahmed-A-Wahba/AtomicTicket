using AtomicTicket.Infrastructure.Outbox;
using AtomicTicket.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace AtomicTicket.Infrastructure.Persistence.Write.Interceptors;

internal sealed class OutboxDomainEventInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var outboxMessages = context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Select(x => x.Entity)
                .SelectMany(x => x.GetDomainEvents())
                .Select(e => new OutboxMessage
                {
                    Id = Guid.CreateVersion7(),
                    OccurredOnUtc = DateTime.UtcNow,
                    Type = e.GetType().Name,
                    Content = JsonConvert.SerializeObject(
                        e,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All
                        }),
                    EventKind = EventKind.Domain
                })
                .ToList();

        if (outboxMessages.Any())
        {
            context.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
