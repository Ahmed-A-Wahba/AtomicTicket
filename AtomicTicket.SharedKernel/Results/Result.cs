namespace AtomicTicket.SharedKernel.Results;

public class Result
{
    private readonly List<Error> _errors = [];
    protected Result() { }
    protected Result(Error error) => _errors.Add(error);
    protected Result(IEnumerable<Error> errors) => _errors.AddRange(errors);

    public bool IsSuccess => _errors.Count == 0;
    public bool IsFailure => !IsSuccess;
    public Error[] Errors => [.. _errors];

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);
    public static Result Failure(IEnumerable<Error> errors) => new(errors);
    public static Result<Unit> SuccessUnit() => new(Unit.Value);

    public static implicit operator Result(Error error) => Failure(error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value) : base() => _value = value;
    protected internal Result(Error error) : base(error) { }
    protected internal Result(IEnumerable<Error> errors) : base(errors) { }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failure result.");

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(Error error) => new(error);
    public static implicit operator Result<TValue>(Error[] errors) => new(errors);
    public static implicit operator Result<TValue>(List<Error> errors) => new(errors);

    public TNextValue Match<TNextValue>(
        Func<TValue, TNextValue> onSuccess,
        Func<Error[], TNextValue> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Errors);
    }
}