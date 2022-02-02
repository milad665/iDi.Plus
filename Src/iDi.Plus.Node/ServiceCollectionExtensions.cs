﻿using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace iDi.Plus.Node;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainService(this IServiceCollection services)
    {
        services.AddScoped<IBlockchainUpdateService, BlockchainUpdateService>();
        services.AddScoped<IIdTransactionFactory, IdTransactionFactory>();
        return services;
    }
}