using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.Domain.Events.DomainEvents;

public sealed record EventCancelledDomainEvent(
    Guid EventId,
    string Reason
) : IDomainEvent;
