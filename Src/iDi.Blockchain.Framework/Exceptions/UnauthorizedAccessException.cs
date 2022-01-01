using System;

namespace iDi.Blockchain.Framework.Exceptions;

public class UnauthorizedAccessException : Exception, IIdPlusException
{
    public UnauthorizedAccessException(string message) : base(message)
    {
        
    }
}