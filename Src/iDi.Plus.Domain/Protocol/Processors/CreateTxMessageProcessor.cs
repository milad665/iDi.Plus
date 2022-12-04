using System;
using System.Linq;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Exceptions;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Entities;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class CreateTxMessageProcessor : MessageProcessorBase
{
    private readonly IIdTransactionFactory _idTransactionFactory;
    private readonly CryptoServiceProvider _cryptoServiceProvider;
    private readonly IConsentRepository _consentRepository;
    
    public CreateTxMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IIdBlockchainRepository idBlockchainRepository, IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider,
        IBlockchainNodesRepository blockchainNodesRepository, IIdTransactionFactory idTransactionFactory, 
        IConsentRepository consentRepository)
        : base(blockchainNodeClient, idBlockchainRepository, hotPoolRepository, localNodeContextProvider,
            blockchainNodesRepository)
    {
        _idTransactionFactory = idTransactionFactory;
        _consentRepository = consentRepository;
        _cryptoServiceProvider = new CryptoServiceProvider();
    }

    public override MessageTypes MessageType => MessageTypes.CreateTx;
    protected override Message ProcessPayload(Message message)
    {
        if (message.Payload is not CreateTxPayload payload)
            throw new InvalidInputException("Payload can not be cast to the target type of this processor.");

        var txData = DecryptPayload(payload);
        if (txData.TransactionType == TransactionTypes.IssueTransaction)
        {
            VerifyIssueTx(payload.SenderPublicKey, txData);
            var doubleEncryptedTxData = DoubleEncryptPayloadByPublicKey(payload.EncryptedTransactionData, txData.HolderAddress.Bytes);
            txData.DoubleEncryptedData = doubleEncryptedTxData;
            if (LocalNodeContextProvider.IsWitnessNode)
            {
                if (TransactionExistsInHotPool(txData.TransactionHash))
                    return null;
                var idTransaction = CreateIdTransactionFromTxData(txData);
                HotPoolRepository.AddTransaction(idTransaction);
            }
        }
        else if (txData.TransactionType == TransactionTypes.ConsentTransaction)
        {
            var consentRequest = VerifyConsentTx(payload.SenderPublicKey, txData);
            if (LocalNodeContextProvider.IsWitnessNode)
            {
                var doubleEncryptedTxData = DoubleEncryptPayloadByPublicKey(payload.EncryptedTransactionData, txData.VerifierAddress.Bytes);
                txData.DoubleEncryptedData = doubleEncryptedTxData;
                var idTransaction = CreateIdTransactionFromTxData(txData);
                StoreConsentTx((ConsentIdTransaction) idTransaction, consentRequest);
            }
        }
        else
            throw new InvalidInputException("Unsupported transaction type.");

        SendToAllWitnessNodes(message);

        return null;
    }

    private bool TransactionExistsInHotPool(HashValue txHash) => HotPoolRepository.GetTransaction(txHash) != null;
    private IdTransaction CreateIdTransactionFromTxData(TxDataRequestPayload txData)
    {
        var lastTxsInTxChain =
            IdBlockchainRepository.GetLastIssueTransactionInTheVirtualTransactionChain(txData.IssuerAddress,
                txData.HolderAddress, txData.Subject, txData.IdentifierKey);
        
        var idTransaction = _idTransactionFactory.CreateFromTxDataRequestPayload(txData, lastTxsInTxChain?.TransactionHash);
        if (idTransaction == null)
            throw new InvalidInputException("Id transaction cannot be created from the payload data.");

        return idTransaction;
    }
    private void SendToAllWitnessNodes(Message message)
    {
        var witnessNodes = BlockchainNodesRepository.GetWitnessNodes()
            .Where(n => !n.NodeId.Equals(message.Header.NodeId)).ToList();

        foreach (var node in witnessNodes)
            SendToNode(node.NodeId, message);
    }
    private TxDataRequestPayload DecryptPayload(CreateTxPayload payload)
    {
        var senderPublicKey = payload.SenderPublicKey;
        
        var decryptedData = _cryptoServiceProvider.DecryptByPublicKey(payload.EncryptedTransactionData, senderPublicKey);
        var txDataPayload = new TxDataRequestPayload(decryptedData);

        return txDataPayload;
    }
    private byte[] DoubleEncryptPayloadByPublicKey(byte[] singleEncryptedData, byte[] encryptionPublicKey)
    {
        return _cryptoServiceProvider.EncryptByPublicKey(singleEncryptedData, encryptionPublicKey);
    }
    private void VerifyIssueTx(byte[] issuerPublicKey, TxDataRequestPayload txDataResponsePayload)
    {
        var issuerAddress = new AddressValue(issuerPublicKey);

        if (!txDataResponsePayload.IssuerAddress.Equals(issuerAddress))
            throw new UnauthorizedException("Cannot verify transaction sender.");
    }
    private ConsentRequest VerifyConsentTx(byte[] holderPublicKey, TxDataRequestPayload txDataRequestPayload)
    {
        var holderAddress = new AddressValue(holderPublicKey);

        if (!txDataRequestPayload.HolderAddress.Equals(holderAddress))
            throw new UnauthorizedException("Cannot verify transaction sender.");

        var correspondingRequest = _consentRepository.GetConsentRequest(txDataRequestPayload.VerifierAddress.HexString,
            txDataRequestPayload.Subject, txDataRequestPayload.IdentifierKey);
        if (correspondingRequest == null)
            throw new UnauthorizedException("Cannot find corresponding consent request.");
        
        return correspondingRequest;
    }

    private void StoreConsentTx(ConsentIdTransaction consentIdTransaction, ConsentRequest consentRequest)
    {
        var consentTx = new ConsentTx(consentIdTransaction.TransactionHash.HexString,
            consentIdTransaction.HolderAddress.HexString, consentIdTransaction.DoubleEncryptedData,
            consentIdTransaction.Timestamp, consentRequest);
        _consentRepository.AddConsent(consentTx);
    }
}