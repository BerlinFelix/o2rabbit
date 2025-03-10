using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;

namespace o2rabbit.BizLog.InternalAbstractions;

internal interface ISpaceValidator
{
    ValidationResult ValidateNewSpace(NewSpaceCommand command);

    ValidationResult ValidateUpdatedSpace(UpdateSpaceCommand command);
}