using AtomicTicket.Contracts.DTOs;

namespace AtomicTicket.Application.Common.Interfaces.Repositories;

public interface IReadEventRepository
{
    Task<EventDto?> GetbyId(Guid id, CancellationToken cancellationToken = default);
}
