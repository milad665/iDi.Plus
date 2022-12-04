using iDi.Blockchain.Framework.Cryptography;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Entities;
using iDi.Plus.Infrastructure.VolatileContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace iDi.Plus.Infrastructure.Repositories;

public class ConsentRepository : IConsentRepository
{
    private readonly VolatileDbContext _context;

    public void AddConsentRequest(ConsentRequest consentRequest)
    {
        _context.ConsentRequests.Add(consentRequest);
        _context.SaveChanges();
    }

    public ConsentRequest GetConsentRequestById(long consentRequestId) =>
        _context.ConsentRequests.FirstOrDefault(r => r.Id == consentRequestId);

    public ConsentRequest GetConsentRequest(string verifierPublicKey, string subject, string identifier) =>
        _context.ConsentRequests.FirstOrDefault(r =>
            r.VerifierPublicKey.Equals(verifierPublicKey) &&
            r.Subject.Equals(subject, StringComparison.InvariantCultureIgnoreCase) &&
            r.Identifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));

    public IEnumerable<ConsentRequest> GetAllActiveVerifierConsentRequests(string verifierPublicKey) =>
        _context.ConsentRequests.Where(r => r.VerifierPublicKey.Equals(verifierPublicKey)).ToList();

    public IEnumerable<ConsentRequest> GetConsentRequestsForHolder(string holderPublicKey) =>
        _context.ConsentRequests.Where(r => r.HolderPublicKey.Equals(holderPublicKey)).ToList();


    public void PurgeConsentRequests(TimeSpan timeToLive)
    {
        var consentRequests = _context.ConsentRequests.Where(r => r.RequestTimeStamp.AddSeconds(timeToLive.TotalSeconds) < DateTime.UtcNow)
            .ToList();
        _context.ConsentRequests.RemoveRange(consentRequests);
        _context.SaveChanges();
    }

    public void AddConsent(ConsentTx consentTx)
    {
        _context.Consents.Add(consentTx);
        _context.SaveChanges();
    }

    public ConsentTx GetConsentByTransactionHash(HashValue transactionHash) =>
        _context.Consents.FirstOrDefault(c =>
            c.ConsentTransactionHash.Equals(transactionHash.HexString, StringComparison.CurrentCultureIgnoreCase));

    public IEnumerable<ConsentTx> GetAllActiveConsentsForVerifier(string verifierPublicKey) =>
        _context.Consents.Include(c => c.Request).Where(c =>
            c.Request.VerifierPublicKey.Equals(verifierPublicKey, StringComparison.CurrentCultureIgnoreCase)).ToList();

    public void PurgeConsents(TimeSpan timeToLive)
    {
        var consents = _context.Consents.Where(r => r.TimeStamp.AddSeconds(timeToLive.TotalSeconds) < DateTime.UtcNow)
            .ToList();
        _context.Consents.RemoveRange(consents);
        _context.SaveChanges();
    }
}