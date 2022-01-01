using System;
using iDi.Blockchain.Framework.Exceptions;

namespace iDi.Plus.Domain.Exceptions
{
    public class HashMismatchIdPlusException :Exception, IIdPlusException
    {
        public HashMismatchIdPlusException(string message) : base(message)
        {
            
        }
    }
}