using System;

namespace iDi.Blockchain.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BinaryMessageHashDataAttribute : BinaryMessageAttributeBase
    {
        public BinaryMessageHashDataAttribute(int order) : base(order)
        {
            ByteLength = Cryptography.HashAlgorithm.HashSize / 8;
        }

        public int ByteLength { get; }
    }
}