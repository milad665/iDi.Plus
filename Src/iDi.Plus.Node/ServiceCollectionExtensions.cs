﻿using iDi.Plus.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace iDi.Plus.Node;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUpdateService(this IServiceCollection services)
    {
        services.AddScoped<IBlockchainUpdateService, BlockchainUpdateService>();

        return services;
    }
}