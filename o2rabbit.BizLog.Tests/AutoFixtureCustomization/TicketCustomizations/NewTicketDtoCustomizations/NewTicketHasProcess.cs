using AutoFixture;
using o2rabbit.BizLog.Abstractions.Models;

namespace o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations.NewTicketDtoCustomizations;

public class NewTicketHasProcess : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<NewTicketDto>(composer =>
        {
            return composer
                .With(x => x.ProcessId);
        });
    }
}