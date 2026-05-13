using AtomicTicket.Domain.Events;
using AtomicTicket.Domain.Repositories;

namespace AtomicTicket.Infrastructure.Persistence.Write.Repositories;

internal class EventRepository(ApplicationDbContext context) : IEventRepository
{
    public void Add(Event @event) => context.Events.Add(@event);
}
