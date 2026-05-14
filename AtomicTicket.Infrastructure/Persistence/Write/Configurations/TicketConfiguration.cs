using AtomicTicket.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtomicTicket.Infrastructure.Persistence.Write.Configurations;

internal sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {

        builder.ToTable("Tickets");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
               .HasColumnName("Id")
               .HasColumnType("uniqueidentifier")
               .ValueGeneratedNever();

        builder.Property<Guid>("EventId")
               .HasColumnName("EventId")
               .HasColumnType("uniqueidentifier")
               .IsRequired();

        builder.HasIndex("EventId")
               .HasDatabaseName("IX_Tickets_EventId");

        builder.Property(t => t.RowVersion)
               .HasColumnName("RowVersion")
               .IsRowVersion()
               .IsConcurrencyToken();

        builder.Property(t => t.Type)
               .HasColumnName("Type")
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(t => t.TotalQuantity)
               .HasColumnName("TotalQuantity")
               .IsRequired();

        builder.Property(t => t.Remaining)
               .HasColumnName("Remaining")
               .IsRequired();

        builder.Property(t => t.IsAvailable)
               .HasColumnName("IsAvailable")
               .IsRequired();


        builder.OwnsOne(t => t.Price, money =>
        {
            money.Property(m => m.Amount)
                 .HasColumnName("Price_Amount")
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();

            money.Property(m => m.Currency)
                 .HasColumnName("Price_Currency")
                 .HasMaxLength(3)
                 .IsRequired();
        });

        // ── Unique Constraint ────────────────────────────────────────────────

        // One ticket type per event — mirrors the domain invariant in Event.AddTicket():
        // "A {type} ticket tier already exists for this event."
        // The DB constraint is the last line of defence if two concurrent inserts slip through.

        builder.HasIndex("EventId", nameof(Ticket.Type))
               .IsUnique()
               .HasDatabaseName("UIX_Tickets_EventId_Type");
    }
}