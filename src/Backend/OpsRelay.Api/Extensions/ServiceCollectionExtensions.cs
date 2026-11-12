using System.Globalization;
using System.Text.Json.Serialization;
using FluentValidation;
using OpsRelay.Infrastructure.Persistence;

namespace OpsRelay.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.GetCultureInfo("en-US");

        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddEndpointsApiExplorer();
        services.AddSwaggerDocumentation();
        services.AddProblemDetails();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");

        services.AddHealthChecks()
            .AddSqlServer(
                connectionString,
                name: "sqlserver",
                timeout: TimeSpan.FromSeconds(5))
            .AddDbContextCheck<OpsRelayDbContext>(name: "dbcontext");

        return services;
    }
}
