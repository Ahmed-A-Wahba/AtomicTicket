using AtomicTicket.Infrastructure.Persistence.Read;
using AtomicTicket.SharedKernel.Guards;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicTicket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddPersistence(configuration);
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(sp =>
        {
            var connectionString = configuration.GetConnectionString("ReadModelConnection");
            Guard.AgainstNullOrEmpty(connectionString);
            var dbName = configuration["MongoSettings:DatabaseName"];
            Guard.AgainstNullOrEmpty(dbName);

            return new MongoDbContext(connectionString!, dbName!);
        });

        return services;
    }
}
