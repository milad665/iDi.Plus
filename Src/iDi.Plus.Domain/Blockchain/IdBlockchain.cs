using System;
using iDi.Blockchain.Framework;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Blockchain
{
    public class IdBlockchain : Blockchain<IdTransaction>
    {
        public IdBlockchain()
        {
        }

        public override void ProofOfWork(Block<IdTransaction> block)
        {
            var difficulty = (int)Math.Round(Math.Log(Blocks.Count, 500000)) + 1;
            while (!block.Hash.EndsWith(new string('0', difficulty)) || !block.Hash.Contains($"1{new string('5', difficulty)}"))
                block.NextNonce();
        }
    }
}
