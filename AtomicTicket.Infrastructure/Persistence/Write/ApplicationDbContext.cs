using AtomicTicket.Application.Abstractions;
using AtomicTicket.Domain.Events;
using AtomicTicket.Infrastructure.Outbox;
using AtomicTicket.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AtomicTicket.Infrastructure.Persistence.Write;

internal sealed class ApplicationDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    public DbSet<Event> Events { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
        => SaveChangesAsync(cancellationToken);

    public void AddIntegrationEvent(IIntegrationEvent integrationEvent)
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.CreateVersion7(),
            OccurredOnUtc = DateTime.UtcNow,
            Type = integrationEvent.GetType().FullName!,

            Content = JsonConvert.SerializeObject(
            integrationEvent,
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            }),

            EventKind = EventKind.Integration
        };

        Set<OutboxMessage>().Add(outboxMessage);
    }
}