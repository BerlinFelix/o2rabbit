using FluentResults;
using FluentValidation.Results;

namespace o2rabbit.Core.ResultErrors;

public class ValidationNotSuccessfulError : Error
{
    public ValidationNotSuccessfulError() : base("Validation not successful.")
    {
    }

    public ValidationNotSuccessfulError(ValidationResult validationResult)
        : base(string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage)))
    {
    }
}