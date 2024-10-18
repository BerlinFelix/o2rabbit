using Microsoft.EntityFrameworkCore;
using o2rabbit.Models;

namespace o2rabbit.BizLog.Context;

public class DbProcessesContext: DbContext
{
    public DbProcessesContext(DbContextOptions<DbProcessesContext> options) : base(options)
    {
        
    }
    public DbSet<Process> Processes { get; set; }
    
    
    
}