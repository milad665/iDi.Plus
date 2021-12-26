using System;

namespace iDi.Blockchain.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BinaryMessageCollectionAttribute : BinaryMessageAttributeBase
    {
        public BinaryMessageCollectionAttribute(int order) : base(order)
        {
        }
    }
}