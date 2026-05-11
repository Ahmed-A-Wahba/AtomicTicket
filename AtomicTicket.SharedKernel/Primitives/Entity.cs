namespace AtomicTicket.SharedKernel.Primitives;

public abstract class Entity<TKey>
{
    public TKey Id { get; init; } = default!;
}
