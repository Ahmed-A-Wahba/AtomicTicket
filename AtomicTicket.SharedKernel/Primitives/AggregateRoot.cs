using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.SharedKernel.Primitives;

public abstract class AggregateRoot<TKey> : Entity<TKey>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        var copy = _domainEvents.ToList();
        _domainEvents.Clear();
        return copy;
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
