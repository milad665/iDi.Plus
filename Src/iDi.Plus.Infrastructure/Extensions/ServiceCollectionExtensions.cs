using iDi.Blockchain.Framework.Blockchain;
using iDi.Plus.Infrastructure.Context;
using iDi.Plus.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace iDi.Plus.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdiInfrastructureServices(this IServiceCollection services, string blockchainMongoDatabaseConnectionString)
        {
            services.AddTransient<IBlockchainContext>((x) => new BlockchainContext(blockchainMongoDatabaseConnectionString));
            services.AddTransient<IBlockchainRepository, BlockchainRepository>();

            return services;
        }

    }
}
