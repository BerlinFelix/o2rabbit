using FluentValidation.Results;
using o2rabbit.BizLog.Abstractions.Models.ProcessModels;

namespace o2rabbit.BizLog.InternalAbstractions;

internal interface IProcessValidator
{
    ValidationResult ValidateNewProcess(NewProcessCommand command);

    ValidationResult ValidateUpdatedProcess(UpdateProcessCommand command);
}