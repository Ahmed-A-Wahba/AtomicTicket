using AtomicTicket.Domain.Events.DomainEvents;
using AtomicTicket.SharedKernel.Primitives;
using AtomicTicket.SharedKernel.Results;

namespace AtomicTicket.Domain.Events;

public sealed class Event : AggregateRoot<Guid>
{
    public Guid UserId { get; init; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Venue Venue { get; private set; }
    public EventStatus Status { get; private set; }
    public DateTimeOffset Date { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private readonly List<Ticket> _tickets = [];
    public IReadOnlyList<Ticket> Tickets => _tickets.AsReadOnly();

    public static Result<Event> Create(
        Guid userId,
        string title,
        string description,
        Venue venue,
        DateTimeOffset date)
    {

        if (string.IsNullOrWhiteSpace(title))
            return Error.Validation("Event.TitleRequired", "Event title cannot be empty.");

        if (title.Length > 200)
            return Error.Validation("Event.TitleTooLong", "Event title cannot exceed 200 characters.");

        if (string.IsNullOrWhiteSpace(description))
            return Error.Validation("Event.DescriptionRequired", "Event description cannot be empty.");

        if (description.Length > 2000)
            return Error.Validation("Event.DescriptionTooLong", "Event description cannot exceed 2000 characters.");

        if (venue is null)
            return Error.Validation("Event.VenueRequired", "A valid venue must be provided.");

        if (date <= DateTimeOffset.UtcNow)
            return Error.Validation("Event.InvalidDate", "Event date must be in the future.");

        var eventAggregate = new Event(
            userId,
            title.Trim(),
            description.Trim(),
            venue,
            date);

        eventAggregate.RaiseDomainEvent(
            new EventCreatedDomainEvent(
                eventAggregate.Id,
                eventAggregate.UserId,
                eventAggregate.Title,
                eventAggregate.Description,
                eventAggregate.Venue,
                eventAggregate.Date
            ));

        return eventAggregate;
    }

    public Result<Ticket> AddTicket(TicketType type, Money price, int quantity)
    {
        if (Status is EventStatus.Cancelled or EventStatus.Completed)
            return Error.Conflict("Event.InvalidStatus", "Cannot add tickets to a cancelled or completed event.");

        if (_tickets.Exists(t => t.Type == type))
            return Error.Validation("Event.TicketTypeExists", $"A {type} ticket tier already exists for this event.");

        int proposedTotal = TotalRemainingTickets + quantity;
        if (proposedTotal > Venue.Capacity)
            return Error.Validation("Event.ExceedsVenueCapacity",
                $"Total ticket quantity ({proposedTotal}) would exceed venue capacity ({Venue.Capacity}).");

        var ticketResult = Ticket.Create(type, price, quantity);
        if (ticketResult.IsFailure)
            return ticketResult.Errors;

        _tickets.Add(ticketResult.Value);
        RaiseDomainEvent(new TicketAddedDomainEvent(Id, ticketResult.Value.Id, type));

        return ticketResult.Value;
    }

    public Result Publish()
    {
        if (Status != EventStatus.Draft)
            return Error.Conflict("Event.NotDraft", "Only draft events can be published.");

        if (!HasAvailableTickets)
            return Error.Validation("Event.NoTickets", "An event must have at least one ticket type before publishing.");

        if (Date <= DateTimeOffset.UtcNow)
            return Error.Validation("Event.PastDate", "Cannot publish an event whose date is in the past.");

        Status = EventStatus.Published;
        RaiseDomainEvent(new EventPublishedDomainEvent(Id, Title));
        return Result.Success();
    }

    public Result Cancel(string reason)
    {
        if (Status is EventStatus.Cancelled)
            return Error.Conflict("Event.AlreadyCancelled", "Event is already cancelled.");

        if (Status is EventStatus.Completed)
            return Error.Validation("Event.AlreadyCompleted", "A completed event cannot be cancelled.");

        if (string.IsNullOrWhiteSpace(reason))
            return Error.Validation("Event.CancellationReasonRequired", "A cancellation reason must be provided.");

        Status = EventStatus.Cancelled;
        _tickets.ForEach(t => t.MarkAsUnavailable());
        RaiseDomainEvent(new EventCancelledDomainEvent(Id, reason.Trim()));
        return Result.Success();
    }

    public Result ReserveTicket(TicketType type, int count = 1)
    {
        if (Status != EventStatus.Published)
            return Error.Conflict("Event.NotPublished", "Tickets can only be reserved for published events.");

        var ticket = _tickets.Find(t => t.Type == type);
        if (ticket is null)
            return Error.NotFound("Ticket.NotFound", $"Ticket type {type} was not found for this event.");

        var reserveResult = ticket.Reserve(count);
        if (reserveResult.IsFailure)
            return reserveResult;

        RaiseDomainEvent(new TicketAvailabilityChangedDomainEvent(Id, ticket.Id, ticket.IsAvailable));

        if (!HasAvailableTickets)
            Status = EventStatus.SoldOut;

        return Result.Success();
    }

    public Result ReleaseTicket(TicketType type, int count = 1)
    {
        if (Status is EventStatus.Cancelled or EventStatus.Completed)
            return Error.Conflict("Event.InvalidStatus", "Cannot release tickets for a cancelled or completed event.");

        var ticket = _tickets.Find(t => t.Type == type);
        if (ticket is null)
            return Error.NotFound("Ticket.NotFound", $"Ticket type {type} was not found.");

        var releaseResult = ticket.Release(count);
        if (releaseResult.IsFailure)
            return releaseResult;

        RaiseDomainEvent(new TicketAvailabilityChangedDomainEvent(Id, ticket.Id, ticket.IsAvailable));

        if (Status == EventStatus.SoldOut)
            Status = EventStatus.Published;

        return Result.Success();
    }

    public Result UpdateVenue(Venue newVenue)
    {
        if (Status is EventStatus.Published or EventStatus.SoldOut)
            return Error.Validation("Event.AlreadyLive", "Venue cannot be changed once the event is live.");

        int totalCommitted = _tickets.Sum(t => t.TotalQuantity);
        if (newVenue.Capacity < totalCommitted)
            return Error.Validation("Venue.InsufficientCapacity",
                $"New venue capacity ({newVenue.Capacity}) is less than total tickets already created ({totalCommitted}).");

        Venue = newVenue;
        return Result.Success();
    }

    public bool HasAvailableTickets => _tickets.Exists(t => t.IsAvailable);
    public int TotalRemainingTickets => _tickets.Sum(t => t.Remaining);

    private Event(
        Guid userId,
        string title,
        string description,
        Venue venue,
        DateTimeOffset date)
        : base(Guid.CreateVersion7())
    {
        UserId = userId;
        Title = title;
        Description = description;
        Venue = venue;
        Date = date;
        Status = EventStatus.Draft;
    }
}
