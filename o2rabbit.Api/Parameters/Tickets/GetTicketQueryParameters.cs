namespace o2rabbit.Api.Parameters.Tickets;

public class GetTicketQueryParameters
{
    public bool IncludeChildren { get; set; }

    public bool IncludeParents { get; set; }

    public bool IncludeComments { get; set; }
}