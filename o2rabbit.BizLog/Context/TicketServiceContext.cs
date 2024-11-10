using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Context;

public class TicketServiceContext : DbContext
{
    private readonly IOptions<TicketServiceContextOptions> _options;

    //For mocking
    public TicketServiceContext()
    {
    }

    public TicketServiceContext(IOptions<TicketServiceContextOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
    }

    public virtual DbSet<Process> Processes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_options.Value.ConnectionString);
    }
}