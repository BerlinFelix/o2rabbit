namespace o2rabbit.Api.Parameters.Search;

public class SearchQueryParameters
{
    public string SearchText { get; set; } = string.Empty;

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}