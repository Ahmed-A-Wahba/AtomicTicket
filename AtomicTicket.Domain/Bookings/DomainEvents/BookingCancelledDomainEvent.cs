using AtomicTicket.SharedKernel.Domain;

namespace AtomicTicket.Domain.Bookings.DomainEvents
{
    public record BookingCancelledDomainEvent(
        Guid BookingId,
        Guid EventId,
        Guid TicketId,
        DateTimeOffset CancelledAt) : IDomainEvent;
}
