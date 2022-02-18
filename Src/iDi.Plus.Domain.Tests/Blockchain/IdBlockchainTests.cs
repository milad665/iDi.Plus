using System.Collections.Generic;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Entities;
using Moq;
using Xunit;

namespace iDi.Plus.Domain.Tests.Blockchain;

public class IdBlockchainTests
{
    private readonly SampleDataProvider _sampleDataProvider;
    private Mock<IIdBlockchainRepository> IdBlockchainRepositoryMock => new Mock<IIdBlockchainRepository>();

    public IdBlockchainTests()
    {
        _sampleDataProvider = new SampleDataProvider();
    }

    [Fact]
    public void PoWProducesCorrectHashAfterNewBlockIsAdded()
    {
        var repo = IdBlockchainRepositoryMock;

        repo.Setup(r => r.GetBlocksCount()).Returns(1);
        repo.Setup(r => r.GetLastBlock()).Returns(Block<IdTransaction>.Genesis);
        repo.Setup(r => r.GetKeyChangeHistory(It.IsAny<AddressValue>())).Returns(new List<KeyChange>());

        var target = new IdBlockchain(repo.Object);
        var block = target.CreateBlock(_sampleDataProvider.GetSampleIdTransactions());
        Assert.EndsWith(new string('0', target.Difficulty), block.Hash.HexString);
    }
}