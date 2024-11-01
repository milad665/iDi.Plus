﻿using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
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

        services.AddScoped<IMessageProcessor, BlockDataMessageProcessor>();
        services.AddScoped<IMessageProcessor, CreateTxMessageProcessor>();
        services.AddScoped<IMessageProcessor, GetBlockMessageProcessor>();
        services.AddScoped<IMessageProcessor, RequestBlockchainUpdateMessageProcessor>();
        services.AddScoped<IMessageProcessor, GetTxMessageProcessor>();
        services.AddScoped<IMessageProcessor, GetWitnessNodesMessageProcessor>();
        services.AddScoped<IMessageProcessor, NewBlocksMessageProcessor>();
        services.AddScoped<IMessageProcessor, HotPoolTxsMessageProcessor>();
        services.AddScoped<IMessageProcessor, TxDataResponseMessageProcessor>();
        services.AddScoped<IMessageProcessor, VoteMessageProcessor>();
        services.AddScoped<IMessageProcessor, WitnessNodesListMessageProcessor>();

        services.AddScoped<IMessageFactory, MessageFactory>();
        services.AddScoped<IConsensusService, ConsensusService>();
        services.AddScoped<IBlockchain<IdTransaction>, IdBlockchain>();

        return services;
    }
}