using iDi.Blockchain.Framework.Entities;
using System;

namespace iDi.Plus.Domain.Entities
{
    public class KeyChange : EntityBase
    {
        public KeyChange()
        {}

        public KeyChange(string newAddress, byte[] newPublicKey, string oldAddress, byte[] oldPublicKey, DateTime changeTime)
        {
            NewAddress = newAddress;
            NewPublicKey = newPublicKey;
            OldAddress = oldAddress;
            OldPublicKey = oldPublicKey;
            ChangeTime = changeTime;
        }

        public string NewAddress { get; private set; }
        public byte[] NewPublicKey { get; private set; }
        public string OldAddress { get; private set; }
        public byte[] OldPublicKey { get; private set; }
        public DateTime ChangeTime { get; private set; }
    }
}
