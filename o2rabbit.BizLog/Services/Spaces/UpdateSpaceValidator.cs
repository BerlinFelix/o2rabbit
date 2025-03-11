using FluentValidation;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.BizLog.Context;

namespace o2rabbit.BizLog.Services.Spaces;

public class UpdateSpaceValidator : AbstractValidator<UpdateSpaceCommand>
{
    private readonly DefaultContext _context;

    public UpdateSpaceValidator(DefaultContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;

        RuleFor(t => t).NotNull();
        RuleFor(t => t.Title).NotEmpty().MinimumLength(1).MaximumLength(100);
        RuleFor(t => t.Description).NotEmpty().MaximumLength(5000);
    }
}