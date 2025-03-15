using FluentValidation;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;

namespace o2rabbit.BizLog.Services.Processes;

public class UpdateProcessValidator : AbstractValidator<UpdateProcessCommand>
{
    public UpdateProcessValidator()
    {
        RuleFor(t => t).NotNull();
        RuleFor(t => t.Name).NotEmpty().MinimumLength(1).MaximumLength(100);
        RuleFor(t => t.Description).NotEmpty().MaximumLength(5000);
    }
}