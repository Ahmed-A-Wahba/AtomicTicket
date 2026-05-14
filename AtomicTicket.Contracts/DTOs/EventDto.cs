namespace AtomicTicket.Contracts.DTOs;

public sealed record EventDto
(
    Guid EventId,
    string Title,
    string Description,
    DateTimeOffset ScheduledAt,
    string VenueName,
    string VenueAddress,
    int VenueCapacity,
    string Status,
    List<TicketDto> Tickets
);

public sealed record TicketDto
(
    Guid TicketId,
    string Type,
    decimal Price,
    string Currency,
    int TotalQuantity,
    bool IsAvailable,
    int RemainingQuantity
);
