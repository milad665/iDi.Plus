using iDi.Blockchain.Framework.Entities;
using System;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Plus.Domain.Entities
{
    public class KeyChange : EntityBase
    {
        public KeyChange()
        {}

        public KeyChange(AddressValue newAddress, AddressValue oldAddress, DateTime changeTime)
        {
            NewAddress = newAddress;
            OldAddress = oldAddress;
            ChangeTime = changeTime;
        }

        public AddressValue NewAddress { get; private set; }
        public AddressValue OldAddress { get; private set; }
        public DateTime ChangeTime { get; private set; }
    }
}
