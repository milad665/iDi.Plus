using System;
using iDi.Blockchain.Framework.Exceptions;

namespace iDi.Blockchain.Framework.Protocol.Exceptions
{
    public class InvalidInputException : Exception, IIdPlusException
    {
        public InvalidInputException(string message) : base(message)
        {
            
        }
    }
}