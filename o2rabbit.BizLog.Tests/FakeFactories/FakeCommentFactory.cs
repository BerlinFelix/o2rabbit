using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Tests.FakeFactories;

internal static class FakeCommentFactory
{
    internal static Comment CreateComment()
    {
        return new Comment()
        {
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            Id = 1,
            Text = "comment",
            TicketId = 1,
        };
    }
}