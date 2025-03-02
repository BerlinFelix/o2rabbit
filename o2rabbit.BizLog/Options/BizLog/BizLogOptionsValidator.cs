using FluentValidation;
using Npgsql;

namespace o2rabbit.BizLog.Options.BizLog;

internal class BizLogOptionsOptionsValidator : AbstractValidator<BizLogOptions>
{
    public BizLogOptionsOptionsValidator()
    {
        RuleFor(o => o.ConnectionStringMainDb)
            .NotEmpty()
            .Must(connectionString =>
            {
                using var connection = new NpgsqlConnection(connectionString);
                try
                {
                    connection.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            });
    }
}