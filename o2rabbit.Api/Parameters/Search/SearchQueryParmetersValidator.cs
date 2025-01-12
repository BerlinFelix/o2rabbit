using FluentValidation;

namespace o2rabbit.Api.Parameters.Search;

public class SearchQueryParmetersValidator : AbstractValidator<SearchQueryParameters>
{
    public SearchQueryParmetersValidator()
    {
        RuleFor(o => o.SearchText)
            .MinimumLength(3);

        RuleFor(o => o.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(o => o.PageSize)
            .GreaterThanOrEqualTo(1);
    }

    // public ValidateOptionsResult Validate(string? name, SearchQueryParameters options)
    // {
    //     var validationResult = this.Validate(options);
    //
    //     return validationResult.IsValid
    //         ? ValidateOptionsResult.Success
    //         : ValidateOptionsResult.Fail(validationResult.ToString());
    // }
}