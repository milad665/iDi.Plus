using System;
using System.Collections.Generic;
using System.Linq;

namespace iDi.Blockchain.Framework
{
    public abstract class Blockchain<TTransaction> where TTransaction: ITransaction
    {
        private readonly List<Block<TTransaction>> _blocks;

        protected Blockchain()
        {
            _blocks = new List<Block<TTransaction>> { Genesis() };
        }

        public IReadOnlyCollection<Block<TTransaction>> Blocks => _blocks;

        public Block<TTransaction> Genesis()
        {
            return new Block<TTransaction>(0, "", DateTime.UtcNow, null);
        }

        public void Add(List<TTransaction> transactions)
        {
            var block = new Block<TTransaction>(_blocks.Count, _blocks.Last().Hash, DateTime.UtcNow, transactions);
            ProofOfWork(block);
            _blocks.Add(block);
        }

        public abstract void ProofOfWork(Block<TTransaction> block);
    }
}
