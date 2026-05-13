using AtomicTicket.Infrastructure.Outbox;
using AtomicTicket.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace AtomicTicket.Infrastructure.Persistence.Write.Interceptors;

public sealed class OutboxDomainEventInterceptor : SaveChangesInterceptor
{
    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null) return base.SavedChangesAsync(eventData, result, cancellationToken);

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

        context.Set<OutboxMessage>().AddRange(outboxMessages);

        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
