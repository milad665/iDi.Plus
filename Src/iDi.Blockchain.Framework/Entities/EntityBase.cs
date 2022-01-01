using System;

namespace iDi.Blockchain.Framework.Entities
{
    public abstract class EntityBase
    {
        public EntityBase()
        {
            TimeStamp = DateTime.UtcNow;
        }

        public long Id { get; protected set; }
        public DateTime TimeStamp { get; private set; }
    }
}
