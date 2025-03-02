using FluentValidation;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Options.BizLog;

namespace o2rabbit.BizLog.Options.TicketServiceContext;

internal class TicketServiceContextOptionsConfigurator : IConfigureOptions<TicketServiceContextOptions>,
    IValidateOptions<TicketServiceContextOptions>
{
    private readonly IValidator<TicketServiceContextOptions> _validator;
    private readonly BizLogOptions _bizLogOptions;

    public TicketServiceContextOptionsConfigurator(IValidator<TicketServiceContextOptions> validator,
        IOptions<BizLogOptions> bizLogOptions)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(bizLogOptions);

        _validator = validator;
        _bizLogOptions = bizLogOptions.Value;
    }

    public void Configure(TicketServiceContextOptions options)
    {
        options.ConnectionString = _bizLogOptions.ConnectionStringMainDb;
    }

    public ValidateOptionsResult Validate(string? name, TicketServiceContextOptions options)
    {
        var result = _validator.Validate(options);

        return result.IsValid ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail($"{result}");
    }
}