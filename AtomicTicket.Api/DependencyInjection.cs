using AtomicTicket.Api.Services;
using AtomicTicket.Application.Abstractions;

namespace AtomicTicket.Api;

internal static class DependencyInjection
{
    internal static IServiceCollection AddPresentationLayer(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IClientContextProvider, ClientContextProvider>();

        return services;
    }
}
