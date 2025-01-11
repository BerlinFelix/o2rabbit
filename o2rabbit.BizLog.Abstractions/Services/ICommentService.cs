using FluentResults;
using o2rabbit.BizLog.Abstractions.Models.CommentModels;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Abstractions.Services;

public interface ICommentService
{
    public Task<Result<Comment>> CreateAsync(NewCommentCommand newCommentCommand,
        CancellationToken cancellationToken = default);

    public Task<Result<Comment>> UpdateAsync(UpdateTicketCommand update, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(long id, CancellationToken cancellationToken = default);
}