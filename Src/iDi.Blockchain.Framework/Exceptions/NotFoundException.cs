using System;

namespace iDi.Blockchain.Framework.Exceptions;

public class NotFoundException: Exception,IIdPlusException
{
    public NotFoundException(string message):base(message)
    {
        
    }
}