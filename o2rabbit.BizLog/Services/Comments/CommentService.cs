using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;

namespace o2rabbit.BizLog.Services.Comments;

[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
internal partial class CommentService : ICommentService
{
    private readonly DefaultContext _context;
    private readonly ILogger<CommentService> _logger;
    private readonly ICommentValidator _commentValidator;

    public CommentService(DefaultContext context,
        ILogger<CommentService> logger,
        ICommentValidator commentValidator)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(commentValidator);

        _context = context;
        _logger = logger;
        _commentValidator = commentValidator;
    }
}