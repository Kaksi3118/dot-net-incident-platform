using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpsRelay.Api.Middleware;
using OpsRelay.Application.Common.Behaviors;
using OpsRelay.Application.Common.Interfaces;
using OpsRelay.Application.Incidents.Commands.CreateIncident;
using OpsRelay.Domain.Repositories;
using OpsRelay.Infrastructure.Persistence;
using OpsRelay.Infrastructure.Repositories;
using OpsRelay.Infrastructure.Services;

namespace OpsRelay.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddMediatR(typeof(CreateIncidentCommand).Assembly);
        services.AddValidatorsFromAssembly(typeof(CreateIncidentCommand).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddDbContext<OpsRelayDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<ISlaPolicyRepository, SlaPolicyRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ISlaCalculator, DefaultSlaCalculator>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
