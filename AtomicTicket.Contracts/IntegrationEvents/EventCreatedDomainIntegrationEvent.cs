namespace AtomicTicket.Contracts.IntegrationEvents;

public record EventCreatedDomainIntegrationEvent
(
    Guid EventId,
    Guid UserId,
    string Title,
    string Description,
    string VenueName,
    string VenueAddress,
    int VenueCapacity,
    DateTimeOffset ScheduledAt
);
