using AutoFixture;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;

namespace o2rabbit.Api.Tests.AutoFixtureCustomization.Tickets.UpdatedTicketDtoCustomizations;

public class UpdatedTicketHasNoParentAndNoProcess : ICustomization
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