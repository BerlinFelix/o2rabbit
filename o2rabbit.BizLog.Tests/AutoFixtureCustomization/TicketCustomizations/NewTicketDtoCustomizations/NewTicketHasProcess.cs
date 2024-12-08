using AutoFixture;
using o2rabbit.BizLog.Abstractions.Models.TicketModels;

namespace o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations.NewTicketDtoCustomizations;

public class NewTicketHasProcess : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<NewTicketCommand>(composer =>
        {
            return composer
                .With(x => x.ProcessId);
        });
    }
}