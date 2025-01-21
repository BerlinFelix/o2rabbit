using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Tests.FakeFactories;

internal static class FakeCommentFactory
{
    internal static Comment CreateComment()
    {
        var utcNow = DateTime.UtcNow;
        return new Comment()
        {
            Created = utcNow,
            LastModified = utcNow,
            Id = 1,
            Text = "comment",
            TicketId = 1,
        };
    }
}