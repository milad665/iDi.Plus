using iDi.Blockchain.Framework.Entities;

namespace iDi.Plus.Domain.Entities
{
    public class HotPoolRecord : EntityBase
    {
        public HotPoolRecord()
        {}

        public HotPoolRecord(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; private set; }
    }
}
