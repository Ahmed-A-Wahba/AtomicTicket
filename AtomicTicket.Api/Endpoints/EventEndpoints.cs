using AtomicTicket.Application.Events.Commands.Create;
using AtomicTicket.Application.Events.Queries.Get;
using AtomicTicket.Contracts.Requests;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AtomicTicket.Api.Endpoints;

public sealed class EventEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/events");

        group.MapPost("/", CreateEvent).WithName(nameof(CreateEvent));

        group.MapGet("/{eventId}", GetEvent).WithName(nameof(GetEvent));
    }

    private async Task<IResult> GetEvent(Guid eventId, [FromServices] ISender sender)
    {
        var query = new GetEventQuery(eventId);

        var result = await sender.Send(query);

        return result.Match(Results.Ok, ApiResults.Problem);
    }

    private static async Task<IResult> CreateEvent(CreateEventRequest request, [FromServices] ISender sender)
    {
        var command = new CreateEventCommand(
            request.Title,
            request.Description,
            request.VenueName,
            request.VenueAddress,
            request.VenueCapacity,
            request.Date);

        var result = await sender.Send(command);

        return result.Match(Results.Ok, ApiResults.Problem);
    }

}
