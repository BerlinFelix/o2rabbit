using o2rabbit.Api.Models;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Extensions;

internal static class ProcessExtensions
{
    internal static TinyProcessDto ToTinyDto(this Process process)
    {
        return new TinyProcessDto()
        {
            Id = process.Id,
            Name = process.Name,
        };
    }

    internal static DefaultProcessDto ToDefaultDto(this Process process)
    {
        var dto = new DefaultProcessDto
        {
            Id = process.Id,
            Name = process.Name,
            Description = process.Description,
        };

        dto.SubProcesses.AddRange(process.SubProcesses.Select(p => p.ToTinyDto()));
        dto.PossibleParentProcesses.AddRange(process.PossibleParentProcesses.Select(p => p.ToTinyDto()));
        dto.Comments.AddRange(process.Comments.Select(c => c.ToDto()));
        dto.PossibleSpaces.AddRange(process.PossibleSpaces.Select(s => s.ToTinyDto()));

        return dto;
    }
}