using AutoFixture;
using AutoFixture.AutoMoq;

namespace o2rabbit.BizLog.Tests.AutoFixtureCustomization;

public class AutoMoqFixture : Fixture
{
    public AutoMoqFixture()
    {
        Customize(new AutoMoqCustomization());
    }
}