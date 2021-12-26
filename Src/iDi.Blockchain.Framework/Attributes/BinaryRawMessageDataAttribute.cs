using System;

namespace iDi.Blockchain.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BinaryRawMessageDataAttribute : BinaryMessageAttributeBase
    {
        public BinaryRawMessageDataAttribute(int order):base(order)
        {
            ByteLength = 0;
        }

        public BinaryRawMessageDataAttribute(int order, int byteLength):base(order)
        {
            ByteLength = byteLength;
        }

        public int ByteLength { get; }
    }
}