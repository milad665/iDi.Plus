using System;

namespace iDi.Blockchain.Framework.Exceptions
{
    public class VerificationFailedException :Exception, IIdPlusException
    {
        public VerificationFailedException(string message) : base(message)
        {
            
        }
    }
}