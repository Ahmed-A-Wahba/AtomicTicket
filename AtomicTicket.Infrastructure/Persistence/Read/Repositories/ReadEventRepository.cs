using AtomicTicket.Application.Common.Interfaces.Repositories;
using AtomicTicket.Contracts.DTOs;
using Mapster;
using MongoDB.Driver;

namespace AtomicTicket.Infrastructure.Persistence.Read.Repositories;

internal class ReadEventRepository(MongoDbContext context) : IReadEventRepository
{
    public async Task<EventDto?> GetbyId(Guid id, CancellationToken cancellationToken)
    {
        var eventModel = await context.Events.Find(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);

        return eventModel.Adapt<EventDto>();
    }
}
