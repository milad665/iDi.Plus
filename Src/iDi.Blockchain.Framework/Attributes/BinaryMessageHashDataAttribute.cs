using iDi.Blockchain.Framework.Cryptography;
using System;

namespace iDi.Blockchain.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BinaryMessageHashDataAttribute : BinaryMessageAttributeBase
    {
        public BinaryMessageHashDataAttribute(int order) : base(order)
        {
            ByteLength = FrameworkEnvironment.HashAlgorithm.HashSize / 8;
        }

        public int ByteLength { get; }
    }
}