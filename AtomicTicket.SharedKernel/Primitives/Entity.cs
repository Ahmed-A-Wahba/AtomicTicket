namespace AtomicTicket.SharedKernel.Primitives;

public abstract class Entity<TKey>(TKey id) where TKey : notnull
{
    public TKey Id { get; init; } = id;

    public override bool Equals(object? obj) =>
        obj is Entity<TKey> other && other.GetType() == GetType() && EqualityComparer<TKey>.Default.Equals(Id, other.Id);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
