using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Extensions;

internal static class SpaceExtensions
{
    internal static Space ToSpace(this NewSpaceCommand command)
    {
        var created = DateTimeOffset.Now;
        return new Space
        {
            Id = 0,
            Title = command.Title,
            Description = command.Description,
            Created = created,
            LastModified = created
        };
    }
}