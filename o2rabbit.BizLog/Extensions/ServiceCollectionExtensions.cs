using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.Options;
using o2rabbit.BizLog.Options.BizLog;
using o2rabbit.BizLog.Options.ProcessService;
using o2rabbit.BizLog.Services;

namespace o2rabbit.BizLog.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBizLog(this IServiceCollection services,
        Action<BizLogOptions, IServiceProvider> action)
    {
        services.AddOptions()
            .AddOptions<BizLogOptions>()
            .Configure<IServiceProvider>(action);

        services.ConfigureOptions<ProcessServiceContextOptionsConfigurator>()
            .ConfigureOptions<ProcessServiceContextOptionsConfigurator>();

        services.AddLogging()
            .AddScoped<IProcessService, ProcessService>()
            .AddDbContext<ProcessServiceContext>();

        services.AddValidatorsFromAssemblyContaining(typeof(_FluentValidationDIRegistrationHook),
            ServiceLifetime.Scoped, includeInternalTypes: true);
        return services;
    }
}