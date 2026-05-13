using AtomicTicket.Domain.Events;
using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.Domain.Bookings.DomainEvents
{
    public record BookingCreatedDomainEvent(
        Guid BookingId,
        Guid eventId,
        Guid ticketId,
        Money PriceSnapshot,
        Seat Seat,
        DateTimeOffset CreatedAt) : IDomainEvent;
}
