using FluentValidation;
using o2rabbit.Utilities.Extensions;

namespace o2rabbit.BizLog.Options.CommentServiceContext;

internal class CommentServiceContextOptionsValidator : AbstractValidator<CommentServiceContextOptions>
{
    public CommentServiceContextOptionsValidator()
    {
        RuleFor(o => o.ConnectionString).NotEmpty().MustConnectToDatabase();
    }
}