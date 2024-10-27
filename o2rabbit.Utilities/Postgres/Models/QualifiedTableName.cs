using System.Diagnostics.CodeAnalysis;

namespace o2rabbit.Utilities.Postgres.Models;

public class QualifiedTableName
{
    private string _table = null!;
    private string _schema = null!;
    
    [SetsRequiredMembers]
    public QualifiedTableName(string schema, string table)
    {
        Schema = schema;
        Table = table;
    }

    public required string Table
    {
        get => _table;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"'{nameof(value)}' cannot be null or whitespace.", nameof(value));
            }

            _table = value;
        }
    }


    public required string Schema
    {
        get => _schema;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"'{nameof(value)}' cannot be null or whitespace.", nameof(value));
            }

            _schema = value;
        }
    }

    public override string ToString() => $"\"{Schema}\".\"{Table}\"";
}