using System;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Blockchain
{
    public class IdBlockchain : BlockchainBase<IdTransaction>
    {
        private const int MaxDifficulty = 20;

        public IdBlockchain()
        {
        }

        protected override void ProofOfWork(Block<IdTransaction> block)
        {
            Difficulty = Math.Min(Blocks.Count / 1000000 + 4, MaxDifficulty);
            while (!block.Hash.HexString.EndsWith(new string('0', Difficulty)))
                block.NextNonce();
        }

        public override int Difficulty { get; protected set; }
    }
}
