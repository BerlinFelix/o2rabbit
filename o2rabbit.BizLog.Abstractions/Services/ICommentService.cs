using FluentResults;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Abstractions.Services;

public interface ICommentService
{
    public Task<Result<TicketComment>> CreateAsync(NewCommentCommand newCommentCommand,
        CancellationToken cancellationToken = default);

    public Task<Result<TicketComment>> UpdateAsync(UpdateCommentCommand update,
        CancellationToken cancellationToken = default);

    Task<Result<TicketComment>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}