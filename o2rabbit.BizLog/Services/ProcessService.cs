using FluentResults;
using o2rabbit.BizLog.Context;
using o2rabbit.Models;

namespace o2rabbit.BizLog.Services;

internal class ProcessService
{
    private readonly ProcessServiceContext _context;

    public ProcessService(ProcessServiceContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        _context = context;
    }
    internal async Task<Result<Process>> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Process>> CreateAsync(Process? process)
    {
        if (process == null) return Result.Fail<Process>("Process is null");
        throw new NotImplementedException();
    }
}