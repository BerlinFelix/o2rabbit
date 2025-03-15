using o2rabbit.BizLog.Abstractions.Models.ProcessModels;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Extensions;

public static class ProcessExtensions
{
    public static Process ToProcess(this NewProcessCommand command)
    {
        var process = new Process()
        {
            Id = 0,
            Name = command.Name,
            Description = command.Description
        };

        return process;
    }
}