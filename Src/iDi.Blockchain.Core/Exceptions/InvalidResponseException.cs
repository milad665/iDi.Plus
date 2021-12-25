using System;

namespace iDi.Blockchain.Core.Exceptions
{
    public class InvalidResponseException : Exception, IDomainException
    {
        public InvalidResponseException(string message): base(message)
        {

        }
    }
}
