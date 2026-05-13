using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.SharedKernel.Primitives;

public interface IHasDomainEvents
{
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}
