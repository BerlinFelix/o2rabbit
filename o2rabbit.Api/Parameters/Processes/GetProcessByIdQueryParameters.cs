namespace o2rabbit.Api.Parameters.Processes;

public class GetProcessByIdQueryParameters
{
    public bool IncludeTickets { get; set; }

    public bool IncludeComments { get; set; }

    public bool IncludeSubProcesses { get; set; }

    public bool IncludePossibleParentProcesses { get; set; }
}