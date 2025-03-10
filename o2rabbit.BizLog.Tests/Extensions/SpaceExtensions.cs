using FluentAssertions;
using o2rabbit.BizLog.Abstractions.Models.SpaceModels;
using o2rabbit.BizLog.Extensions;

namespace o2rabbit.BizLog.Tests.Extensions;

public class SpaceExtensions
{
    [Fact]
    public void NewSpaceCommand_ToSpace_IsEquivalentToCommand()
    {
        var command = new NewSpaceCommand()
        {
            Title = "Sample Title",
            Description = "Sample Description"
        };

        var result = command.ToSpace();

        result.Should().BeEquivalentTo(command);
    }
}