using FluentValidation;
using Microsoft.Extensions.Options;

namespace o2rabbit.BizLog.Options.ProcessService;

internal class ProcessServiceContextOptionsConfigurator : IConfigureOptions<ProcessServiceContextOptions>,
    IValidateOptions<ProcessServiceContextOptions>
{
    private readonly IValidator<ProcessServiceContextOptions> _validator;

    public ProcessServiceContextOptionsConfigurator(IValidator<ProcessServiceContextOptions> validator)
    {
        ArgumentNullException.ThrowIfNull(validator);

        _validator = validator;
    }

    public void Configure(ProcessServiceContextOptions options)
    {
    }

    public ValidateOptionsResult Validate(string? name, ProcessServiceContextOptions options)
    {
        var result = _validator.Validate(options);

        return result.IsValid ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail($"{result}");
    }
}