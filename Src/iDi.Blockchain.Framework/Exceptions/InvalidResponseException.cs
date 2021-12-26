using System;

namespace iDi.Blockchain.Framework.Exceptions
{
    public class InvalidResponseException : Exception, IDomainException
    {
        public InvalidResponseException(string message): base(message)
        {

        }
    }
}
