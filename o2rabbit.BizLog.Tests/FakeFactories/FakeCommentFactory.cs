using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Tests.FakeFactories;

internal static class FakeCommentFactory
{
    internal static TicketComment CreateComment()
    {
        var utcNow = DateTime.UtcNow;
        return new TicketComment()
        {
            Created = utcNow,
            LastModified = utcNow,
            Id = 1,
            Text = "comment",
            TicketId = 1,
        };
    }
}