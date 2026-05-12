using AtomicTicket.SharedKernel.Primitives;
using AtomicTicket.SharedKernel.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicTicket.Domain.Bookings
{
    public sealed record Seat : ValueObject
    {
        public char Row { get; init; }
        public int Number { get; init; }

        private Seat(char row, int number)
        {
            Row = row;
            Number = number;
        }

        public static Result<Seat> Create(char row, int number)
        {
            if (!char.IsLetter(row))
                return Error.Validation("Seat.InvalidRow", "Row must be a letter.");

            if (number <= 0)
                return Error.Validation("Seat.InvalidNumber", "Seat number must be greater than zero.");

            return new Seat(char.ToUpperInvariant(row), number);
        }

        public override string ToString() => $"{Row}{Number}"; 
    }
}
