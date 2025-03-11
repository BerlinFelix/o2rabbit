using FluentValidation;
using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.BizLog.InternalAbstractions;

namespace o2rabbit.BizLog.Services.Spaces;

public class SpaceValidator : ISpaceValidator
{
    private readonly IValidator<NewSpaceCommand> _newSpaceValidator;
    private readonly IValidator<UpdateSpaceCommand> _updateSpaceValidator;

    public SpaceValidator(IValidator<NewSpaceCommand> newSpaceValidator,
        IValidator<UpdateSpaceCommand> updateSpaceValidator)
    {
        ArgumentNullException.ThrowIfNull(newSpaceValidator);
        ArgumentNullException.ThrowIfNull(updateSpaceValidator);

        _newSpaceValidator = newSpaceValidator;
        _updateSpaceValidator = updateSpaceValidator;
    }

    public ValidationResult ValidateNewSpace(NewSpaceCommand command) => _newSpaceValidator.Validate(command);

    public ValidationResult ValidateUpdatedSpace(UpdateSpaceCommand command) => _updateSpaceValidator.Validate(command);
}