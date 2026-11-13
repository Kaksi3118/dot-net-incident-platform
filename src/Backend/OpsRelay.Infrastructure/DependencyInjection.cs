using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Domain.Repositories;
using OpsRelay.Domain.Services;
using OpsRelay.Infrastructure.Events;
using OpsRelay.Infrastructure.Persistence;
using OpsRelay.Infrastructure.Repositories;
using OpsRelay.Infrastructure.Services;

namespace OpsRelay.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");

        services.AddSingleton(TimeProvider.System);

        services.AddDbContext<OpsRelayDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sqlOptions =>
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<ISlaPolicyRepository, SlaPolicyRepository>();
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<ISlaCalculator, StandardBusinessHoursSlaCalculator>();
        services.AddSingleton<ISlaCalculator, Critical24x7SlaCalculator>();
        services.AddSingleton<ISlaCalculatorFactory, SlaCalculatorFactory>();

        return services;
    }
}
