using AtomicTicket.Application.Abstractions.Messaging;
using AtomicTicket.SharedKernel.Results;

namespace AtomicTicket.Application.Events.Commands.Create;

public sealed record CreateEventCommand
(
    string Title,
    string Description,
    string VenueName,
    string VenueAddress,
    int VenueCapacity,
    DateTimeOffset Date
) : ICommand<Result<Guid>>;
