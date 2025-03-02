using FluentValidation;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Options.BizLog;

namespace o2rabbit.BizLog.Options.CommentServiceContext;

internal class CommentServiceContextOptionsConfigurator : IConfigureOptions<CommentServiceContextOptions>,
    IValidateOptions<CommentServiceContextOptions>
{
    private readonly IValidator<CommentServiceContextOptions> _validator;
    private readonly BizLogOptions _bizLogOptions;

    public CommentServiceContextOptionsConfigurator(IValidator<CommentServiceContextOptions> validator,
        IOptions<BizLogOptions> bizLogOptions)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(bizLogOptions);

        _validator = validator;
        _bizLogOptions = bizLogOptions.Value;
    }

    public void Configure(CommentServiceContextOptions options)
    {
        options.ConnectionString = _bizLogOptions.ConnectionStringMainDb;
    }

    public ValidateOptionsResult Validate(string? name, CommentServiceContextOptions options)
    {
        var result = _validator.Validate(options);

        return result.IsValid ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail($"{result}");
    }
}