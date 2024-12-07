using AutoFixture;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;

namespace o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations.NewTicketDtoCustomizations;

public class NewTicketHasNoProcessAndNoParent : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<NewTicketDto>(composer =>
        {
            return composer
                .Without(x => x.ProcessId)
                .Without(x => x.ParentId);
        });
    }
}