using AutoFixture;
using AutoFixture.AutoMoq;

namespace o2rabbit.Api.Tests.AutoFixtureCustomization;

public class AutoMoqFixture : Fixture
{
    public AutoMoqFixture()
    {
        Customize(new AutoMoqCustomization());
    }
}