namespace o2rabbit.BizLog.Abstractions.Options;

public class GetProcessByIdOptions
{
    public bool IncludeTickets { get; set; }

    public bool IncludeComments { get; set; }

    public bool IncludeSubProcesses { get; set; }

    public bool IncludePossibleParentProcesses { get; set; }
}