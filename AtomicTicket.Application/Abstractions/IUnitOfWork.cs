using AtomicTicket.SharedKernel.Primitives;

namespace AtomicTicket.Application.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellation = default);
    void AddIntegrationEvent(IIntegrationEvent integrationEvent);
}
