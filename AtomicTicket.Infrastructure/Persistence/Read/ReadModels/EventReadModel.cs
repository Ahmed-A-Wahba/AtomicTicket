using MongoDB.Bson.Serialization.Attributes;

namespace AtomicTicket.Infrastructure.Persistence.Read.ReadModels;

public class EventReadModel
{
    [BsonId]
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset ScheduledAt { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string VenueAddress { get; set; } = string.Empty;
    public int VenueCapacity { get; set; }
    public string Status { get; set; } = string.Empty;

    public List<TicketSummary> Tickets { get; set; } = [];
}

public class TicketSummary
{
    public Guid TicketId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public int RemainingQuantity { get; set; }
}