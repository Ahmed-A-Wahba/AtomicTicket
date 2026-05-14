using AtomicTicket.Infrastructure.Persistence.Read.ReadModels;
using MongoDB.Driver;

namespace AtomicTicket.Infrastructure.Persistence.Read;

public class MongoDbContext(IMongoClient client, string databaseName)
{
    private readonly IMongoDatabase _database = client.GetDatabase(databaseName);

    public IMongoCollection<EventReadModel> Events => _database.GetCollection<EventReadModel>("Events");
}
