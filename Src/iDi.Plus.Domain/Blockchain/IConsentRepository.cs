using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Plus.Domain.Entities;

namespace iDi.Plus.Domain.Blockchain;

public interface IConsentRepository
{
    void AddConsentRequest(ConsentRequest consentRequest);
    ConsentRequest GetConsentRequestById(long consentRequestId);
    ConsentRequest GetConsentRequest(string verifierPublicKey, string subject, string identifier);
    IEnumerable<ConsentRequest> GetAllActiveVerifierConsentRequests(string verifierPublicKey);
    IEnumerable<ConsentRequest> GetConsentRequestsForHolder(string holderPublicKey);
    void PurgeConsentRequests(TimeSpan timeToLive);

    void AddConsent(ConsentTx consentTx);
    ConsentTx GetConsentByTransactionHash(HashValue transactionHash);
    IEnumerable<ConsentTx> GetAllActiveConsentsForVerifier(string verifierPublicKey);
    void PurgeConsents(TimeSpan timeToLive);
}