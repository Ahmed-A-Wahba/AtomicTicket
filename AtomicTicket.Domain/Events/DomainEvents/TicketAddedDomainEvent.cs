using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.Domain.Events.DomainEvents;

public sealed record TicketAddedDomainEvent(
    Guid EventId,
    Guid TicketId,
    TicketType Type
) : IDomainEvent;
