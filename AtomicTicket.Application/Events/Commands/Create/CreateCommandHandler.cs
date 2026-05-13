using AtomicTicket.Application.Abstractions;
using AtomicTicket.Application.Abstractions.Messaging;
using AtomicTicket.Domain.Events;
using AtomicTicket.Domain.Repositories;
using AtomicTicket.SharedKernel.Results;

namespace AtomicTicket.Application.Events.Commands.Create;

internal sealed class CreateCommandHandler
(
    IEventRepository eventRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContext
) : ICommandHandler<CreateEventCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        var userId = clientContext.UserId();

        var venueResult = Venue.Create(command.VenueName, command.VenueAddress, command.VenueCapacity);

        if (venueResult.IsFailure) return venueResult.Errors;

        var eventResult = Event.Create(
            userId,
            command.Title,
            command.Description,
            venueResult.Value,
            command.Date
            );

        if (eventResult.IsFailure) return eventResult.Errors;

        eventRepository.Add(eventResult.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return eventResult.Value.Id;
    }
}
