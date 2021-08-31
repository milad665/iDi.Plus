using System;
using iDi.Blockchain.Core.Exceptions;

namespace iDi.Protocol.iDiDirect.Exceptions
{
    public class InvalidDataException : Exception, IDomainException
    {
        public InvalidDataException(string message) : base(message)
        {
            
        }
    }
}