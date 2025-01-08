using FluentValidation;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Abstractions.Options;

namespace o2rabbit.BizLog.Options.Search;

internal class SearchOptionsValidator : AbstractValidator<SearchOptions>, IValidateOptions<SearchOptions>
{
    public SearchOptionsValidator()
    {
        RuleFor(o => o.SearchText)
            .MinimumLength(3);

        RuleFor(o => o.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(o => o.PageSize)
            .GreaterThanOrEqualTo(1);
    }

    public ValidateOptionsResult Validate(string? name, SearchOptions options)
    {
        var validationResult = this.Validate(options);

        return validationResult.IsValid
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(validationResult.ToString());
    }
}