using System;

namespace iDi.Blockchain.Core.Attributes
{
    public abstract class BinaryMessageAttributeBase : Attribute
    {
        public BinaryMessageAttributeBase(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }
}