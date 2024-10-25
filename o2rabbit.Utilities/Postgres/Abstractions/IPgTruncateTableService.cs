namespace o2rabbit.Utilities.Postgres.Abstractions;

public interface IPgTruncateTableService
{
   Task TruncateTableAsync(string tableName, CancellationToken cancellationToken = default); 
}