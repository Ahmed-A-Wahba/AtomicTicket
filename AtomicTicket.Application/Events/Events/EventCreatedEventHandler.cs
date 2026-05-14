using AtomicTicket.Application.Abstractions;
using AtomicTicket.Contracts.IntegrationEvents;
using AtomicTicket.Domain.Events;
using AtomicTicket.Domain.Events.DomainEvents;
using MediatR;

namespace AtomicTicket.Application.Events.Events;

internal sealed class EventCreatedDomainEventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<EventCreatedDomainEvent>
{
    public async Task Handle(
        EventCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var integrationEvent = new EventCreatedIntegrationEvent(
            notification.EventId,
            notification.UserId,
            notification.Title,
            notification.Description,
            notification.VenueName,
            notification.VenueAddress,
            notification.VenueCapacity,
            EventStatus.Draft.ToString(),
            notification.ScheduledAt);

        unitOfWork.AddIntegrationEvent(integrationEvent);

        await Task.CompletedTask;
    }
}