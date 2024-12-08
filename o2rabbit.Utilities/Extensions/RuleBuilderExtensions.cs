using FluentValidation;
using Npgsql;

namespace o2rabbit.Utilities.Extensions;

public static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, TProperty> MustConnectToDatabase<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        return ruleBuilder.Must(connectionString =>
        {
            if (connectionString is not string)
            {
                return false;
            }

            try
            {
                using var connection = new NpgsqlConnection(connectionString as string);
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        });
    }

    // TODO MusConnectToDatabasAsync
    // TODO IHierarchicalEntity MustNotHaveCircularDependency
}