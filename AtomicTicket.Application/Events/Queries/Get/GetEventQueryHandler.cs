using AtomicTicket.Application.Abstractions.Messaging;
using AtomicTicket.Application.Common.Interfaces.Repositories;
using AtomicTicket.Contracts.DTOs;
using AtomicTicket.SharedKernel.Results;

namespace AtomicTicket.Application.Events.Queries.Get;

internal sealed class GetEventQueryHandler(IReadEventRepository readEventRepository)
    : IQueryHandler<GetEventQuery, Result<EventDto>>
{
    public async Task<Result<EventDto>> Handle(GetEventQuery query, CancellationToken cancellationToken)
    {
        var @event = await readEventRepository.GetbyId(query.EventId, cancellationToken);

        if (@event is null) return Error.NotFound();

        return @event;
    }
}
