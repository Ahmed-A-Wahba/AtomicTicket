using AtomicTicket.Contracts.IntegrationEvents;
using AtomicTicket.Infrastructure.Persistence.Read;
using AtomicTicket.Infrastructure.Persistence.Read.ReadModels;
using MassTransit;
using MongoDB.Driver;

namespace AtomicTicket.Infrastructure.Messaging.Consumers;

public sealed class EventCreatedConumer(MongoDbContext dbContext) : IConsumer<EventCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<EventCreatedIntegrationEvent> context)
    {
        var integrationEvent = context.Message;

        var readModel = new EventReadModel
        {
            Id = integrationEvent.EventId,
            Title = integrationEvent.Title,
            Description = integrationEvent.Description,
            ScheduledAt = integrationEvent.ScheduledAt,
            VenueName = integrationEvent.VenueName,
            VenueAddress = integrationEvent.VenueAddress,
            VenueCapacity = integrationEvent.VenueCapacity,
            Status = integrationEvent.Status,
            Tickets = []
        };

        await dbContext.Events.ReplaceOneAsync(
            filter: x => x.Id == integrationEvent.EventId,
            replacement: readModel,
            options: new ReplaceOptions { IsUpsert = true },
            cancellationToken: context.CancellationToken);
    }
}
