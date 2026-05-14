using AtomicTicket.Application.Abstractions.Messaging;
using AtomicTicket.Contracts.DTOs;
using AtomicTicket.SharedKernel.Results;

namespace AtomicTicket.Application.Events.Queries.Get;

public sealed record GetEventQuery
(
    Guid EventId
) : IQuery<Result<EventDto>>;
