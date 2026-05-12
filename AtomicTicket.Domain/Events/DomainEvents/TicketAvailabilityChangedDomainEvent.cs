using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.Domain.Events.DomainEvents;

public sealed record TicketAvailabilityChangedDomainEvent(
    Guid EventId,
    Guid TicketId,
    bool IsAvailable
) : IDomainEvent;
