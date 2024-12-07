using AutoFixture;

namespace o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations.UpdatedTicketDto;

public class UpdatedTicketHasNoParent : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Abstractions.Models.TicketModels.UpdatedTicketDto>(composer =>
        {
            return composer
                .Without(x => x.ParentId);
        });
    }
}