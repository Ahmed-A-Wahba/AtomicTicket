using AtomicTicket.Application.Abstractions;
using AtomicTicket.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace AtomicTicket.Infrastructure.Persistence.Write;

internal class ApplicationDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
