using FluentValidation;
using Microsoft.Extensions.Options;

namespace o2rabbit.Api.Options.Connection;

internal class ConnectionStringOptionsConfigurator : IConfigureOptions<ConnectionStringOptions>,
    IValidateOptions<ConnectionStringOptions>
{
    private readonly IValidator<ConnectionStringOptions> _validator;

    public ConnectionStringOptionsConfigurator(IValidator<ConnectionStringOptions> validator)
    {
        ArgumentNullException.ThrowIfNull(validator);

        _validator = validator;
    }

    public void Configure(ConnectionStringOptions options)
    {
    }

    public ValidateOptionsResult Validate(string? name, ConnectionStringOptions options)
    {
        var result = _validator.Validate(options);

        return result.IsValid ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail($"{result}");
    }
}