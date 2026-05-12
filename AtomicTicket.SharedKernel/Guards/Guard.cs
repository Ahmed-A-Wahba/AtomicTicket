namespace AtomicTicket.SharedKernel.Guards;

public static class Guard
{
    public static void AgainstNull<T>(T value, string parameterName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }
}
