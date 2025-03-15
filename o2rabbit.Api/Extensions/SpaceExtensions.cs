using o2rabbit.Api.Models;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Extensions;

internal static class SpaceExtensions
{
    internal static TinySpaceDto ToTinyDto(this Space space)
    {
        var dto = new TinySpaceDto()
        {
            Id = space.Id,
            Title = space.Title,
        };

        return dto;
    }

    internal static DefaultSpaceDto ToDefaultDto(this Space space)
    {
        var dto = new DefaultSpaceDto()
        {
            Id = space.Id,
            Title = space.Title,
            Description = space.Description,
            Created = space.Created,
            LastModified = space.LastModified
        };
        dto.AttachedTickets.AddRange(space.AttachedTickets.Select(t => t.ToTinyTicketDto()));
        dto.AttachableProcesses.AddRange(space.AttachableProcesses.Select(p => p.ToTinyDto()));
        dto.Comments.AddRange(space.Comments.Select(c => c.ToDto()));

        return dto;
    }
}