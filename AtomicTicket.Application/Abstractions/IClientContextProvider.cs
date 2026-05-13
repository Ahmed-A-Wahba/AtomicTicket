namespace AtomicTicket.Application.Abstractions;

public interface IClientContextProvider
{
    Guid UserId();
}
