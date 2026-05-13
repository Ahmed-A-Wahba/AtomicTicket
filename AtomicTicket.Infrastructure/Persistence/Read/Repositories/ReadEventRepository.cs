using AtomicTicket.Infrastructure.Persistence.Read.ReadModels;
using MongoDB.Driver;

namespace AtomicTicket.Infrastructure.Persistence.Read.Repositories;

internal class ReadEventRepository(MongoDbContext context)
{
    public async Task<EventReadModel?> GetbyId(Guid id) => await context.Events.FindSync(e => e.Id == id).FirstOrDefaultAsync();
}
