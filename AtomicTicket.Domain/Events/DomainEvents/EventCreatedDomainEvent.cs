using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.Domain.Events.DomainEvents;

public sealed record EventCreatedDomainEvent
(
    Guid EventId,
    Guid UserId,
    string Title,
    string Description,
    string VenueName,
    string VenueAddress,
    int VenueCapacity,
    DateTimeOffset ScheduledAt
) : IDomainEvent;
