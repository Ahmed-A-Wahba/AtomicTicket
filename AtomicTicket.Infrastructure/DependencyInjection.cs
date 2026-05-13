using AtomicTicket.Domain.Repositories;
using AtomicTicket.Infrastructure.Persistence.Read;
using AtomicTicket.Infrastructure.Persistence.Write;
using AtomicTicket.Infrastructure.Persistence.Write.Interceptors;
using AtomicTicket.Infrastructure.Persistence.Write.Repositories;
using AtomicTicket.SharedKernel.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AtomicTicket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddPersistence(configuration);
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {


        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetService<OutboxDomainEventInterceptor>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            Guard.AgainstNullOrEmpty(connectionString);

            options.UseSqlServer(connectionString)
                .AddInterceptors(interceptor!);
        });
        services.AddSingleton<OutboxDomainEventInterceptor>();


        services.AddSingleton(sp =>
        {
            var connectionString = configuration.GetConnectionString("ReadModelConnection");
            Guard.AgainstNullOrEmpty(connectionString);

            var dbName = configuration["MongoSettings:DatabaseName"];
            Guard.AgainstNullOrEmpty(dbName);

            return new MongoDbContext(connectionString!, dbName!);
        });


        services.AddScoped<IEventRepository, EventRepository>();

        return services;
    }
}
