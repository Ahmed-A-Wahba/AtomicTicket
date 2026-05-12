namespace AtomicTicket.Domain.Events;

public enum EventStatus
{
    Draft,
    Published,
    SoldOut,
    Cancelled,
    Completed
}
