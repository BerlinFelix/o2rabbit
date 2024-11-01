using FluentResults;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.Core;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Abstractions.Services;

public interface IProcessService
{
    public Task<Result<Process>> CreateAsync(Process process,
        CancellationToken cancellationToken = default);

    public Task<Result<Process>> GetByIdAsync(long id, GetByIdOptions? options = null, CancellationToken cancellationToken = default);
    
    public Task<Result<Process>> UpdateAsync(Process process, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(long id, CancellationToken cancellationToken = default);
}