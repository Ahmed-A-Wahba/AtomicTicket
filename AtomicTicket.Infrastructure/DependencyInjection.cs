using AtomicTicket.Application.Abstractions;
using AtomicTicket.Application.Common.Interfaces.Repositories;
using AtomicTicket.Domain.Repositories;
using AtomicTicket.Infrastructure.Messaging;
using AtomicTicket.Infrastructure.Messaging.Consumers;
using AtomicTicket.Infrastructure.Outbox;
using AtomicTicket.Infrastructure.Persistence.Read;
using AtomicTicket.Infrastructure.Persistence.Read.Repositories;
using AtomicTicket.Infrastructure.Persistence.Write;
using AtomicTicket.Infrastructure.Persistence.Write.Interceptors;
using AtomicTicket.Infrastructure.Persistence.Write.Repositories;
using AtomicTicket.SharedKernel.Guards;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
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

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = configuration.GetConnectionString("ReadModelConnection");
            Guard.AgainstNullOrEmpty(connectionString);

            return new MongoClient(connectionString);
        });

        services.AddScoped(sp =>
        {
            var dbName = configuration["MongoSettings:DatabaseName"];
            Guard.AgainstNullOrEmpty(dbName);

            var client = sp.GetRequiredService<IMongoClient>();

            return new MongoDbContext(client, dbName!);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IEventRepository, EventRepository>();


        services.AddScoped<IReadEventRepository, ReadEventRepository>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddQuartz(configuration =>
        {
            var jobKey = new JobKey(nameof(DomainEventOutboxProcessor));

            configuration.AddJob<DomainEventOutboxProcessor>(jobKey).AddTrigger(trigger => trigger.ForJob(jobKey).WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(1).RepeatForever()));
        });

        services.AddQuartz(configuration =>
        {
            var jobKey = new JobKey(nameof(IntegrationEventOutboxProcessor));

            configuration.AddJob<IntegrationEventOutboxProcessor>(jobKey).AddTrigger(trigger => trigger.ForJob(jobKey).WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(1).RepeatForever()));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddTransient<IEventBus, MassTransitEventBus>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumers(typeof(DependencyInjection).Assembly);

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost");
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
