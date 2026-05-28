using AtomicTicket.Infrastructure.Outbox;
using AtomicTicket.Infrastructure.Persistence.Write;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System.Linq.Expressions;

[DisallowConcurrentExecution]
internal abstract class OutboxProcessorBase(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessorBase> logger) : IJob
{
    private static readonly JsonSerializerSettings _settings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Processing outbox messages...");
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var messages = await dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null && m.Error == null)
            .Where(GetMessageFilter())
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        if (messages.Count == 0) return;

        foreach (var message in messages)
        {
            try
            {
                await ProcessAndPublishEventAsync(message, scope.ServiceProvider, context.CancellationToken);

                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                message.Error = ex.Message;
            }
        }
        await dbContext.SaveChangesAsync(context.CancellationToken);
    }

    protected abstract Expression<Func<OutboxMessage, bool>> GetMessageFilter();
    protected abstract Task ProcessAndPublishEventAsync(OutboxMessage message, IServiceProvider serviceProvider, CancellationToken cancellationToken);
    protected static object? DeserializeContent(string content) => JsonConvert.DeserializeObject<object>(content, _settings);
}