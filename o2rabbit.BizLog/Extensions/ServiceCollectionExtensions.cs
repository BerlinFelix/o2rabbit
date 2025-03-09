using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;
using o2rabbit.BizLog.Options;
using o2rabbit.BizLog.Options.BizLog;
using o2rabbit.BizLog.Options.ProcessServiceContext;
using o2rabbit.BizLog.Options.Search;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.BizLog.Services.Comments;
using o2rabbit.BizLog.Services.Tickets;
using ProcessService = o2rabbit.BizLog.Services.Processes.ProcessService;

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
            ServiceLifetime.Transient, includeInternalTypes: true);

        services.AddDbContext<DefaultContext>();

        #endregion

        #region processes

        services
            .AddScoped<IProcessService, ProcessService>();
        services.ConfigureOptions<DefaultContextOptionsConfigurator>()
            .ConfigureOptions<DefaultContextOptionsConfigurator>();

        #endregion

        #region tickets

        services
            .AddScoped<ITicketService, TicketService>()
            .AddScoped<ITicketValidator, TicketValidator>()
            .AddScoped<IValidator<UpdateTicketCommand>, UpdatedTicketValidator>()
            .AddScoped<IValidator<NewTicketCommand>, NewTicketValidator>()
            .AddDbContext<TicketServiceContext>();

        services.ConfigureOptions<TicketServiceContextOptionsConfigurator>();

        #endregion

        #region comments

        services
            .AddScoped<ICommentService, CommentService>()
            .AddScoped<ICommentValidator, CommentValidator>()
            // .AddScoped<IValidator<UpdateCommentCommand>, UpdatedCommentValidator>()
            .AddScoped<IValidator<NewCommentCommand>, NewCommentValidator>();

        #endregion

        #region search

        services.ConfigureOptions<SearchOptionsValidator>();

        #endregion

        return services;
    }
}