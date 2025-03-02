namespace o2rabbit.Api.Options.Connection;

public class ConnectionStringOptions
{
    public required string ConnectionStringMainDb { get; init; }
    public required string ConnectionStringUserDb { get; init; }
}