using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Options;
using o2rabbit.Core;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Context;

public class ProcessServiceContext: DbContext
{
    private readonly IOptions<ProcessServiceContextOptions> _options;

    public ProcessServiceContext(IOptions<ProcessServiceContextOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        
        _options = options;
    }
    
    public DbSet<Process> Processes { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_options.Value.ConnectionString);
    }
}