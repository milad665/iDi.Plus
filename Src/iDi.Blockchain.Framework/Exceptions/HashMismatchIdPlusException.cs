using System;

namespace iDi.Blockchain.Framework.Exceptions
{
    public class HashMismatchIdPlusException :Exception, IIdPlusException
    {
        public HashMismatchIdPlusException(string message) : base(message)
        {
            
        }
    }
}