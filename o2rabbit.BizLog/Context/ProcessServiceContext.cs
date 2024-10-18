using Microsoft.EntityFrameworkCore;
using o2rabbit.Models;

namespace o2rabbit.BizLog.Context;

public class ProcessServiceContext: DbContext
{
    public ProcessServiceContext(DbContextOptions<ProcessServiceContext> options) : base(options)
    {
        
    }
    public DbSet<Process> Processes { get; set; }
    
    
    
}