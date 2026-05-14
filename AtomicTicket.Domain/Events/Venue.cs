using AtomicTicket.SharedKernel.Primitives;
using AtomicTicket.SharedKernel.Results;

namespace AtomicTicket.Domain.Events;

public sealed record Venue : ValueObject
{
    public string Name { get; } = default!;
    public string Address { get; } = default!;
    public int Capacity { get; } = default!;


    public static Result<Venue> Create(string name, string address, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Venue.NameRequired", "Venue name cannot be empty.");

        if (string.IsNullOrWhiteSpace(address))
            return Error.Validation("Venue.AddressRequired", "Venue address cannot be empty.");

        if (capacity <= 0)
            return Error.Validation("Venue.InvalidCapacity", "Venue capacity must be greater than zero.");

        return new Venue(name.Trim(), address.Trim(), capacity);
    }

    public override string ToString() => $"{Name} — {Address} (cap. {Capacity:N0})";

    private Venue(string name, string address, int capacity)
    {
        Name = name;
        Address = address;
        Capacity = capacity;
    }

    private Venue() { }
}
