namespace AtomicTicket.SharedKernel.Results;

public readonly record struct Unit
{
    private static readonly Unit _value = new();

    public static ref readonly Unit Value => ref _value;

    public override string ToString() => "()";
}
