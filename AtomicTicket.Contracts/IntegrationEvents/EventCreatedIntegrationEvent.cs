using AtomicTicket.SharedKernel.Primitives;

namespace AtomicTicket.Contracts.IntegrationEvents;

public sealed record EventCreatedIntegrationEvent
(
    Guid EventId,
    Guid UserId,
    string Title,
    string Description,
    string VenueName,
    string VenueAddress,
    int VenueCapacity,
    string Status,
    DateTimeOffset ScheduledAt
) : IIntegrationEvent;