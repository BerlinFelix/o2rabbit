using Microsoft.EntityFrameworkCore;
using o2rabbit.Models;

namespace o2rabbit.Migrations.Context;

public class DefaultContext: DbContext
{
    private readonly string? _connectionString;

    public DefaultContext(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        
        _connectionString = connectionString;
    }
    
    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Add ConnectionString directly when testing.
        if (_connectionString is not null)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
        
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Process> Processes { get; set; }
}