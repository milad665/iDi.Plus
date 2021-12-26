using System;
using iDi.Blockchain.Framework.Exceptions;

namespace iDi.Protocol.iDiDirect.Exceptions
{
    public class InvalidDataException : Exception, IDomainException
    {
        public InvalidDataException(string message) : base(message)
        {
            
        }
    }
}