using AutoFixture;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;

public class TicketHasNoProcessNoParentsNoChildren : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customize<Ticket>(composer =>
        {
            return composer.Without(x => x.ParentId)
                .Without(x => x.Process)
                .Without(x => x.ProcessId);
        });
    }
}