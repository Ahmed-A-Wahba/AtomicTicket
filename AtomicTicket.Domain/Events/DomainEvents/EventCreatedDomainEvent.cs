using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.Domain.Events.DomainEvents;

public sealed record EventCreatedDomainEvent
(
    Guid EventId,
    Guid UserId,
    string Title,
    string Description,
    Venue Venue,
    DateTimeOffset ScheduledAt
) : IDomainEvent;
