using Microsoft.EntityFrameworkCore;
using o2rabbit.Models;

namespace o2rabbit.Migrations.Context;

public class DefaultContext: DbContext
{
    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
        
    }
    
    public DbSet<Process> Processes { get; set; }
}