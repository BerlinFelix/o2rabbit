namespace o2rabbit.BizLog.Abstractions.Options;

public class GetTicketByIdOptions
{
    public bool IncludeChildren { get; set; }

    public bool IncludeParents { get; set; }

    public bool IncludeComments { get; set; }
}