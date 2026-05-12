using AtomicTicket.Domain.Events;
using AtomicTicket.SharedKernel.Domain;
using System;
using System.Collections.Generic;
using System.Text;

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
