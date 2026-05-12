using System.Runtime.CompilerServices;

namespace AtomicTicket.SharedKernel.Guards;

public static class Guard
{
    public static void AgainstNull<T>(T? value,
        [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    public static void AgainstNullOrEmpty(string? value,
        [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be null or empty.", parameterName);
        }
    }
}
