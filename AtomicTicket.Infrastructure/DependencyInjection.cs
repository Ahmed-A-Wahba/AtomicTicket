using AtomicTicket.Domain.Repositories;
using AtomicTicket.Infrastructure.Outbox;
using AtomicTicket.Infrastructure.Persistence.Read;
using AtomicTicket.Infrastructure.Persistence.Write;
using AtomicTicket.Infrastructure.Persistence.Write.Interceptors;
using AtomicTicket.Infrastructure.Persistence.Write.Repositories;
using AtomicTicket.SharedKernel.Guards;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace AtomicTicket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddPersistence(configuration)
            .AddServices();
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

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddQuartz(configuration =>
        {
            var jobKey = new JobKey(nameof(OutboxProcessor));

            configuration.AddJob<OutboxProcessor>(jobKey).AddTrigger(trigger => trigger.ForJob(jobKey).WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(10).RepeatForever()));
        });

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/");
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
