using FluentValidation;
using Microsoft.Extensions.Options;

namespace o2rabbit.BizLog.Options.BizLog;

internal class BizLogOptionsConfigurator : IConfigureOptions<BizLogOptions>, IValidateOptions<BizLogOptions>
{
    private readonly IValidator<BizLogOptions> _validator;

    public BizLogOptionsConfigurator(IValidator<BizLogOptions> validator)
    {
        ArgumentNullException.ThrowIfNull(validator);

        _validator = validator;
    }

    public void Configure(BizLogOptions options)
    {
    }

    public ValidateOptionsResult Validate(string? name, BizLogOptions options)
    {
        var result = _validator.Validate(options);

        return result.IsValid ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail($"{result}");
    }
}