using System;

namespace iDi.Blockchain.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BinaryMessageDataAttribute : BinaryMessageAttributeBase
    {
        public BinaryMessageDataAttribute(int order):base(order)
        {
            ByteLength = 0;
        }

        public BinaryMessageDataAttribute(int order, int byteLength):base(order)
        {
            ByteLength = byteLength;
        }

        public int ByteLength { get; }
    }
}