using System;

namespace iDi.Blockchain.Framework.Exceptions
{
    public class InvalidResponseException : Exception, IIdPlusException
    {
        public InvalidResponseException(string message): base(message)
        {

        }
    }
}
