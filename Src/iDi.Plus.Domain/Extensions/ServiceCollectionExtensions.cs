using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Plus.Domain.Communication;
using iDi.Plus.Domain.Protocol;
using iDi.Plus.Domain.Protocol.Processors;
using iDi.Plus.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace iDi.Plus.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddSingleton<IBlockchainUpdateServer, BlockchainUpdateServer>();
        services.AddScoped<IMessageProcessor, GetNewBlocksMessageProcessor>();
        services.AddScoped<IMessageFactory, MessageFactory>();
        services.AddScoped<IConsensusService, ConsensusService>();
        services.AddScoped<IVerificationService, VerificationService>();

        return services;
    }
}