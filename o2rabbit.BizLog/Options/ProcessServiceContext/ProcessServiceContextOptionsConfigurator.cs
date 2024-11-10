using FluentValidation;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Options.BizLog;
using o2rabbit.BizLog.Options.ProcessServiceContext;

namespace o2rabbit.BizLog.Options.ProcessService;

internal class ProcessServiceContextOptionsConfigurator : IConfigureOptions<ProcessServiceContextOptions>,
    IValidateOptions<ProcessServiceContextOptions>
{
    private readonly IValidator<ProcessServiceContextOptions> _validator;
    private readonly BizLogOptions _bizLogOptions;

    public ProcessServiceContextOptionsConfigurator(IValidator<ProcessServiceContextOptions> validator,
        IOptions<BizLogOptions> bizLogOptions)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(bizLogOptions);

        _validator = validator;
        _bizLogOptions = bizLogOptions.Value;
    }

    public void Configure(ProcessServiceContextOptions options)
    {
        options.ConnectionString = _bizLogOptions.ConnectionString;
    }

    public ValidateOptionsResult Validate(string? name, ProcessServiceContextOptions options)
    {
        var result = _validator.Validate(options);

        return result.IsValid ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail($"{result}");
    }
}