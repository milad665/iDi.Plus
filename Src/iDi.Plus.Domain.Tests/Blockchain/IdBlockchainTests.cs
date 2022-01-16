using System.Linq;
using iDi.Plus.Domain.Blockchain;
using Xunit;

namespace iDi.Plus.Domain.Tests.Blockchain;

public class IdBlockchainTests
{
    private readonly SampleDataProvider _sampleDataProvider;

    public IdBlockchainTests()
    {
        _sampleDataProvider = new SampleDataProvider();
    }

    [Fact]
    public void PoWProducesCorrectHashAfterNewBlockIsAdded()
    {
        var target = new IdBlockchain();
        target.AddBlock(_sampleDataProvider.GetSampleIdTransactions());
        var block = target.Blocks.Last();
        Assert.EndsWith(new string('0', target.Difficulty), block.Hash);
    }
}