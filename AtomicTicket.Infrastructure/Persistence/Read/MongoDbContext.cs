using AtomicTicket.Infrastructure.Persistence.Read.ReadModels;
using MongoDB.Driver;

namespace AtomicTicket.Infrastructure.Persistence.Read;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<EventReadModel> Events => _database.GetCollection<EventReadModel>("Events");
}
