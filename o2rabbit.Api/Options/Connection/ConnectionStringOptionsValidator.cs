using FluentValidation;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.Api.Options.Connection;

internal class ConnectionStringOptionsValidator : AbstractValidator<ConnectionStringOptions>
{
    public ConnectionStringOptionsValidator()
    {
        RuleFor(o => o.ConnectionStringMainDb)
            .NotEmpty()
            .MustConnectToDatabase();

        // RuleFor(o => o.ConnectionStringUserDb)
        //     .NotEmpty()
        //     .MustConnectToDatabase();
    }
}