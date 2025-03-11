namespace o2rabbit.BizLog.Abstractions.Options;

public class GetSpaceByIdOptions
{
    public bool IncludeProcesses { get; set; }

    public bool IncludeTickets { get; set; }

    public bool IncludeComments { get; set; }
}