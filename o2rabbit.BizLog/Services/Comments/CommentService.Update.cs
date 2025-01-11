using FluentResults;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Services.Comments;

internal partial class CommentService
{
    public Task<Result<Comment>> UpdateAsync(UpdateTicketCommand update, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}