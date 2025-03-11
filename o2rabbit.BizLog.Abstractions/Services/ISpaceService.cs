using FluentResults;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Abstractions.Services;

public interface ISpaceService
{
    public Task<Result<Space>> CreateAsync(NewSpaceCommand command,
        CancellationToken cancellationToken = default);

    public Task<Result> DeleteAsync(long id,
        CancellationToken cancellationToken = default);

    public Task<Result<Space>> GetByIdAsync(long id, GetSpaceByIdOptions? options = null,
        CancellationToken cancellationToken = default);
}