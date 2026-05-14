using AtomicTicket.Api.Services;
using AtomicTicket.Application.Abstractions;
using Carter;

namespace AtomicTicket.Api;

internal static class DependencyInjection
{
    internal static IServiceCollection AddPresentationLayer(this IServiceCollection services)
    {
        services.AddCarter();
        services.AddHttpContextAccessor();
        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddOpenApi();

        services.AddScoped<IClientContextProvider, ClientContextProvider>();

        return services;
    }
}
