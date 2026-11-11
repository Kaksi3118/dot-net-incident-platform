using Serilog;

namespace OpsRelay.Api.Extensions;

public static class SerilogExtensions
{
    public static IHostBuilder UseOpsRelaySerilog(this IHostBuilder hostBuilder) =>
        hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());
}
