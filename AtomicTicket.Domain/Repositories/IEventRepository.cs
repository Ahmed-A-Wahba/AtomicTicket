using AtomicTicket.Domain.Events;

namespace AtomicTicket.Domain.Repositories;

public interface IEventRepository
{
    void Add(Event @event);
}
