using FluentValidation;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;

namespace o2rabbit.BizLog.Services.Processes;

public class NewProcessValidator : AbstractValidator<NewProcessCommand>
{
    public NewProcessValidator()
    {
        RuleFor(t => t).NotNull();
        RuleFor(t => t.Name).NotEmpty().MinimumLength(1).MaximumLength(100);
        RuleFor(t => t.Description).NotEmpty().MaximumLength(5000);
        RuleFor(t => t.Workflow).NotNull();
        RuleFor(t => t.Workflow)
            .ChildRules(validator =>
                {
                    validator.RuleFor(workflow => workflow.Statuses)
                        .NotEmpty()
                        .Must(list => list.Any(status => status.IsFinal))
                        .WithMessage("At least one status must be final");
                    validator.RuleFor(workflow => workflow.Statuses)
                        .Must(list =>
                            list.Select(s => s.Name).Count()
                            == list.Select(s => s.Name).Distinct().Count())
                        .WithMessage("Names of statuses must be unique");
                }
            );

        RuleFor(t => t.Workflow)
            .ChildRules(validator =>
                validator.RuleFor(list => list.StatusTransitions)
                    .NotEmpty()
            );
    }
}