namespace iDi.Plus.Domain.Tests.Protocol;

public abstract class ProtocolsTestBase
{
    protected SampleDataProvider SampleDataProvider { get; }

    protected ProtocolsTestBase()
    {
        SampleDataProvider = new SampleDataProvider();
    }
}