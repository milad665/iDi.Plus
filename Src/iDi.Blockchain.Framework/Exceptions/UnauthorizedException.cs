using System;

namespace iDi.Blockchain.Framework.Exceptions;

public class UnauthorizedException : Exception, IIdPlusException
{
    public UnauthorizedException(string message) : base(message)
    {
        
    }
}