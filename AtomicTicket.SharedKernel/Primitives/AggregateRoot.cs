using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.SharedKernel.Primitives;

public abstract class AggregateRoot<TKey>(TKey id) : Entity<TKey>(id) where TKey : notnull, IHasDomainEvents
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
