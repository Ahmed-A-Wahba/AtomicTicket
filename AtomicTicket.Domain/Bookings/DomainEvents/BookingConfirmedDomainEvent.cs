using AtomicTicket.SharedKernel.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicTicket.Domain.Bookings.DomainEvents
{
    public record BookingConfirmedDomainEvent(
        Guid BookingId,
        Guid EventId,
        Guid TicketId,
        DateTimeOffset ConfirmedAt) : IDomainEvent;

}
