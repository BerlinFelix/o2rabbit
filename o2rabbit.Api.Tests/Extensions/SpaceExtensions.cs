using FluentAssertions;
using o2rabbit.Api.Extensions;
using o2rabbit.Core.Entities;

namespace o2rabbit.Api.Tests.Extensions;

public class SpaceExtensions
{
    [Fact]
    public void Space_ToDefaultDto_IsEquivalent()
    {
        var space = new Space
        {
            Id = 0,
            Title = "title",
            Description = "description",
            Created = DateTimeOffset.MinValue,
            LastModified = DateTimeOffset.MinValue
        };

        var defaultDto = space.ToDefaultDto();

        defaultDto.Should().BeEquivalentTo(space, config =>
        {
            config.Excluding(s => s.AttachedTickets)
                .Excluding(s => s.AttachableProcesses)
                .Excluding(s => s.Comments);
            return config;
        });
        defaultDto.AttachedTickets.Count.Should().Be(space.AttachedTickets.Count);
        defaultDto.AttachableProcesses.Count.Should().Be(space.AttachableProcesses.Count);
        defaultDto.Comments.Count.Should().Be(space.Comments.Count);
    }
}