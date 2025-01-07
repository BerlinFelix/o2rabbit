using FluentValidation;

namespace o2rabbit.Api.Options.Search;

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
}