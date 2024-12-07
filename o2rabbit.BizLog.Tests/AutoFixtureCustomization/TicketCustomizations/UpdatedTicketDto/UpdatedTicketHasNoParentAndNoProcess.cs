using AutoFixture;

namespace o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations.UpdatedTicketDto;

public class UpdatedTicketHasNoParentAndNoProcess : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Abstractions.Models.UpdatedTicketDto>(composer =>
        {
            return composer
                .Without(x => x.ProcessId)
                .Without(x => x.ParentId);
        });
    }
}