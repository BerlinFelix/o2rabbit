using FluentValidation;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Options.BizLog;

namespace o2rabbit.BizLog.Options.ProcessServiceContext;

internal class DefaultContextOptionsConfigurator : IConfigureOptions<DefaultContextOptions>,
    IValidateOptions<DefaultContextOptions>
{
    private readonly IValidator<DefaultContextOptions> _validator;
    private readonly BizLogOptions _bizLogOptions;

    public DefaultContextOptionsConfigurator(IValidator<DefaultContextOptions> validator,
        IOptions<BizLogOptions> bizLogOptions)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(bizLogOptions);

        _validator = validator;
        _bizLogOptions = bizLogOptions.Value;
    }

    public void Configure(DefaultContextOptions options)
    {
        options.ConnectionString = _bizLogOptions.ConnectionStringMainDb;
    }

    public ValidateOptionsResult Validate(string? name, DefaultContextOptions options)
    {
        var result = _validator.Validate(options);

        return result.IsValid ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail($"{result}");
    }
}