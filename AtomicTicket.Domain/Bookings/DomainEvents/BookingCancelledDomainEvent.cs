using AtomicTicket.SharedKernel.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicTicket.Domain.Bookings.DomainEvents
{
    public record BookingCancelledDomainEvent(
        Guid BookingId,
        Guid EventId,
        Guid TicketId,
        DateTimeOffset CancelledAt) : IDomainEvent;
}
