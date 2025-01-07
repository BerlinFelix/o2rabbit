namespace o2rabbit.BizLog.Abstractions.Options;

public class SearchOptions
{
    public string SearchText { get; set; } = string.Empty;

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}