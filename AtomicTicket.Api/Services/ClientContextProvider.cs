using AtomicTicket.Application.Abstractions;
using System.Security.Claims;

namespace AtomicTicket.Api.Services;

public class ClientContextProvider(IHttpContextAccessor accessor) : IClientContextProvider
{
    public Guid UserId()
    {
        var userIdString = accessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}
