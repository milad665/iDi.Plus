using System;
using iDi.Blockchain.Framework.Exceptions;

namespace iDi.Blockchain.Framework.Protocol.Exceptions
{
    public class InvalidDataException : Exception, IIdPlusException
    {
        public InvalidDataException(string message) : base(message)
        {
            
        }
    }
}