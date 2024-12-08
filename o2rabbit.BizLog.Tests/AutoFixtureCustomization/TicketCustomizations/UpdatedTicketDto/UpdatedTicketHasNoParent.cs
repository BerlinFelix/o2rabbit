using AutoFixture;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;

namespace o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations.UpdatedTicketDto;

public class UpdatedTicketHasNoParent : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<UpdatedTicketCommand>(composer =>
        {
            return composer
                .Without(x => x.ParentId);
        });
    }
}