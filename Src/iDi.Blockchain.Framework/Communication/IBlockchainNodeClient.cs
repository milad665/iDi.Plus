using System.Net;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Blockchain.Framework.Communication;

public interface IBlockchainNodeClient
{
    Message Send(IPEndPoint remoteEndpoint, Message messageToSend);
}