using AutoFixture;
using o2rabbit.BizLog.Abstractions.Models;

namespace o2rabbit.Api.Tests.AutoFixtureCustomization.Tickets.UpdatedTicketDtoCustomizations;

public class UpdatedTicketHasNoParentAndNoProcess : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<UpdatedTicketDto>(composer =>
        {
            return composer
                .Without(x => x.ProcessId)
                .Without(x => x.ParentId);
        });
    }
}