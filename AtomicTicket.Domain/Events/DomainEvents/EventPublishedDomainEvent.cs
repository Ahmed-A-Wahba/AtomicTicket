using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.Domain.Events.DomainEvents;

public sealed record EventPublishedDomainEvent(
    Guid EventId,
    string EventTitle
) : IDomainEvent;