using o2rabbit.Models;

namespace o2rabbit.BizLog.Abstractions.Services;

public interface IProcessService
{
    public Task<Process> CreateProcessAsync(Process process, CancellationToken cancellationToken = default);
}