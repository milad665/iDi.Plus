using System;
using iDi.Blockchain.Core.Exceptions;

namespace iDi.Plus.Domain.Exceptions
{
    public class HashMismatchDomainException :Exception, IDomainException
    {
        public HashMismatchDomainException(string message) : base(message)
        {
            
        }
    }
}