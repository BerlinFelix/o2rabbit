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
}