using AtomicTicket.Domain.Bookings.DomainEvents;
using AtomicTicket.Domain.Events;
using AtomicTicket.SharedKernel.Primitives;
using AtomicTicket.SharedKernel.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicTicket.Domain.Bookings
{
    public sealed class Booking : AggregateRoot<Guid>
    {
        public Guid EventId { get; private set; }
        public Guid TicketId { get; private set; }

       // public Guid customerId { get; private set; }
        public Money PriceSnapshot { get; private set; }  = default!; // Price at the time of booking the ticket, to handle price changes later
        public BookingStatus Status { get; private set; }
        public Seat Seat { get; private set; } = default!; // في ال Configuration ابقي اعمل الكولوم ده  unique عشان ماحدش يقدر يحجز نفس المقعد مرتين
        public DateTimeOffset CreatedAt { get; private set; }
        public byte[] RowVersion { get; private set; } = [];


        private Booking() : base(Guid.Empty) { }

        private Booking(
            Guid id,
            Guid eventId,
            Guid ticketId,
            Money priceSnapshot,
            Seat seat
            ) : base(id)
        {
            EventId = eventId;
            TicketId = ticketId;
            PriceSnapshot = priceSnapshot;
            Seat = seat;
            Status = BookingStatus.Pending;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public static Result<Booking> Create(
            Guid eventId,
            Guid ticketId,
            Money priceSnapshot,
            Seat seat)
        {
            if (eventId == Guid.Empty)
                return Error.Validation("Booking.InvalidEventId", "EventId cannot be empty.");
            if (ticketId == Guid.Empty)
                return Error.Validation("Booking.InvalidTicketId", "TicketId cannot be empty.");
            if (priceSnapshot == null)
                return Error.Validation("Booking.InvalidPriceSnapshot", "PriceSnapshot cannot be null.");

            var booking = new Booking(
                Guid.NewGuid(),
                eventId,
                ticketId,
                priceSnapshot,
                seat
                );

            booking.RaiseDomainEvent(new BookingCreatedDomainEvent(
                booking.Id,
                booking.EventId,
                booking.TicketId,
                booking.PriceSnapshot,
                booking.Seat,
                booking.CreatedAt
                ));

            return booking;
        }


        public Result Confirm()
        {
            if (Status != BookingStatus.Pending)
                return Error.Conflict("Booking.InvalidStatus", "Only pending bookings can be confirmed.");

            Status = BookingStatus.Confirmed;

            RaiseDomainEvent(new BookingConfirmedDomainEvent(
                Id,
                EventId,
                TicketId,
                CreatedAt
                ));

            return Result.Success();
        }
            


        public Result Csncel()
        {
            if (Status == BookingStatus.Cancelled)
                return Error.Conflict("Booking.AlreadyCancelled", "Booking is already cancelled.");

            Status = BookingStatus.Cancelled;

            RaiseDomainEvent(new BookingCancelledDomainEvent(
                Id,
                EventId,
                TicketId,
                CreatedAt
                ));

            return Result.Success();
        }


    }    
}
