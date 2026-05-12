using AtomicTicket.SharedKernel.Primitives;
using AtomicTicket.SharedKernel.Results;

namespace AtomicTicket.Domain.Events;

public sealed class Ticket : Entity<Guid>
{
    public TicketType Type { get; private set; }
    public Money Price { get; private set; }
    public int TotalQuantity { get; private set; }
    public int Remaining { get; private set; }
    public bool IsAvailable { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private Ticket(
        TicketType type,
        Money price,
        int quantity)
        : base(Guid.CreateVersion7())
    {
        Type = type;
        Price = price;
        TotalQuantity = quantity;
        Remaining = quantity;
        IsAvailable = quantity > 0;
    }


    internal static Result<Ticket> Create(
        TicketType type,
        Money price,
        int quantity)
    {
        if (quantity <= 0)
            return Error.Validation("Ticket.Quantity", "Ticket quantity must be at least 1.");

        var ticket = new Ticket(type, price, quantity);
        return ticket;
    }

    internal Result Reserve(int count = 1)
    {
        if (!IsAvailable || Remaining == 0)
            return Error.Conflict("Ticket.SoldOut", $"Ticket type {Type} is sold out.");

        if (count <= 0)
            return Error.Validation("Ticket.InvalidCount", "Reservation count must be at least 1.");

        if (count > Remaining)
            return Error.Validation(
                "Ticket.InsufficientQuantity",
                $"Only {Remaining} ticket(s) remain for type {Type}.");

        Remaining -= count;
        IsAvailable = Remaining > 0;

        return Result.Success();
    }

    internal Result Release(int count = 1)
    {
        if (Remaining + count > TotalQuantity)
            return Result.Failure(
                Error.Validation("Ticket.Release", "Cannot release more tickets than originally created."));

        Remaining += count;
        IsAvailable = Remaining > 0;

        return Result.Success();
    }

    internal void MarkAsUnavailable()
    {
        IsAvailable = false;
    }
}
