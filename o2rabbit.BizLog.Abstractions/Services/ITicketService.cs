using FluentResults;
using o2rabbit.BizLog.Abstractions.Options;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Abstractions.Services;

public interface ITicketService
{
    public Task<Result<Ticket>> CreateAsync(Ticket ticket,
        CancellationToken cancellationToken = default);

    public Task<Result<Ticket>> GetByIdAsync(long id, GetTicketByIdOptions? options = null,
        CancellationToken cancellationToken = default);

    public Task<Result<Ticket>> UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(long id, CancellationToken cancellationToken = default);

    Task<Result<Ticket>> CreateFromProcessAsync(Process process, CancellationToken cancellationToken = default);
}