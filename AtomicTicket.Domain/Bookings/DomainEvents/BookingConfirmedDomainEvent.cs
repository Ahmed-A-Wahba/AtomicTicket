using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.Domain.Bookings.DomainEvents
{
    public record BookingConfirmedDomainEvent(
        Guid BookingId,
        Guid EventId,
        Guid TicketId,
        DateTimeOffset ConfirmedAt) : IDomainEvent;

}
