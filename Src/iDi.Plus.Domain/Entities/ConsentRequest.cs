using System;
using iDi.Blockchain.Framework.Entities;

namespace iDi.Plus.Domain.Entities;

public class ConsentRequest : EntityBase
{
    private ConsentRequest()
    {}
    
    public ConsentRequest(string verifierPublicKey, string holderPublicKey, string subject, string identifier, DateTime requestTimeStamp)
    {
        VerifierPublicKey = verifierPublicKey;
        HolderPublicKey = holderPublicKey;
        Subject = subject;
        Identifier = identifier;
        RequestTimeStamp = requestTimeStamp;
    }
    
    public string VerifierPublicKey { get; private set; }
    public string HolderPublicKey { get; private set; }
    public string Subject { get; private set; }
    public string Identifier { get; private set; }
    public DateTime RequestTimeStamp { get; private set; }
}