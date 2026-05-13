using AtomicTicket.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtomicTicket.Infrastructure.Persistence.Write.Configurations;

internal sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("Id")
               .HasColumnType("uniqueidentifier")
               .ValueGeneratedNever();
        builder.Property(e => e.UserId)
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property(e => e.RowVersion)
               .HasColumnName("RowVersion")
               .IsRowVersion()
               .IsConcurrencyToken();

        builder.Property(e => e.Title)
               .HasColumnName("Title")
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(e => e.Description)
               .HasColumnName("Description")
               .HasMaxLength(2000)
               .IsRequired();

        builder.Property(e => e.Date)
               .HasColumnName("Date")
               .HasColumnType("datetimeoffset")
               .IsRequired();

        builder.Property(e => e.Status)
               .HasColumnName("Status")
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.OwnsOne(e => e.Venue, venue =>
        {
            venue.Property(v => v.Name)
                 .HasColumnName("Venue_Name")
                 .HasMaxLength(200)
                 .IsRequired();

            venue.Property(v => v.Address)
                 .HasColumnName("Venue_Address")
                 .HasMaxLength(500)
                 .IsRequired();

            venue.Property(v => v.Capacity)
                 .HasColumnName("Venue_Capacity")
                 .IsRequired();
        });


        builder.HasMany(e => e.Tickets)
               .WithOne()
               .HasForeignKey("EventId")
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Tickets)
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasField("_tickets");
    }
}