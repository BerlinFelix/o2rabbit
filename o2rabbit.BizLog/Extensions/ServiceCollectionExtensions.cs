using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using o2rabbit.BizLog.Abstractions;
using o2rabbit.BizLog.Abstractions.Models;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Models;
using o2rabbit.BizLog.Options;
using o2rabbit.BizLog.Options.BizLog;
using o2rabbit.BizLog.Options.ProcessService;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services;
using o2rabbit.BizLog.Services.Tickets;

namespace o2rabbit.BizLog.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBizLog(this IServiceCollection services,
        Action<BizLogOptions, IServiceProvider> action)
    {
        #region shared

        services.AddOptions()
            .AddOptions<BizLogOptions>()
            .Configure<IServiceProvider>(action);
        services.AddLogging();

        services.AddValidatorsFromAssemblyContaining(typeof(_FluentValidationDIRegistrationHook),
            ServiceLifetime.Scoped, includeInternalTypes: true);

        #endregion

        #region processes

        services
            .AddScoped<IProcessService, ProcessService>()
            .AddDbContext<ProcessServiceContext>();
        services.ConfigureOptions<ProcessServiceContextOptionsConfigurator>()
            .ConfigureOptions<ProcessServiceContextOptionsConfigurator>();

        #endregion

        #region tickets

        services
            .AddScoped<ITicketService, TicketService>()
            .AddScoped<ITicketValidator, TicketValidator>()
            .AddScoped<IValidator<TicketUpdate>, UpdatedTicketValidator>()
            .AddScoped<IValidator<NewTicketDto>, NewTicketValidator>()
            .AddDbContext<TicketServiceContext>();

        services.ConfigureOptions<TicketServiceContextOptionsConfigurator>();

        #endregion

        return services;
    }
}