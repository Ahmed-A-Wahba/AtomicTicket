namespace AtomicTicket.Contracts.Requests;

public sealed record CreateEventRequest
(
    string Title,
    string Description,
    string VenueName,
    string VenueAddress,
    int VenueCapacity,
    DateTimeOffset Date
);
