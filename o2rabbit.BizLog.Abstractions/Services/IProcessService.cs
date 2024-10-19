using o2rabbit.Core;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Abstractions.Services;

public interface IProcessService
{
    public Task<Process> CreateProcessAsync(Process process, CancellationToken cancellationToken = default);
}