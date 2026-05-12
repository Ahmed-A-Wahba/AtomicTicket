using AtomicTicket.SharedKernel.Primitives;
using AtomicTicket.SharedKernel.Results;

namespace AtomicTicket.Domain.Events;

public sealed record Money : ValueObject
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = default!;
    public static Result<Money> Create(decimal amount, string currency)
    {
        if (amount < 0)
            return Error.Validation("Money.NegativeAmount", "Amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency))
            return Error.Validation("Money.CurrencyRequired", "Currency is required.");

        var normalised = currency.Trim().ToUpperInvariant();

        if (normalised.Length != 3)
            return Error.Validation("Money.InvalidCurrency", "Currency must be a 3-letter ISO code.");

        return new Money(amount, normalised);
    }

    public Result<Money> Add(Money other)
    {
        if (other.Currency != Currency)
            return Error.Validation(
            "Money.CurrencyMismatch",
            $"Cannot add {other.Currency} to {Currency}. Currencies must match.");

        return new Money(Amount + other.Amount, Currency);
    }

    public static Money Free(string currency) => new(0, currency.ToUpperInvariant());

    public override string ToString() => $"{Amount:F2} {Currency}";

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
}
