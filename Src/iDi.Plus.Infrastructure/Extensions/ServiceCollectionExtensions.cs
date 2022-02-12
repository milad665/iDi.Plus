using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Infrastructure.Context;
using iDi.Plus.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace iDi.Plus.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdiInfrastructureServices(this IServiceCollection services, string blockchainMongoDatabaseConnectionString)
        {
            services.AddScoped<IBlockchainContext>((x) => new BlockchainContext(blockchainMongoDatabaseConnectionString));
            services.AddScoped<IIdBlockchainRepository, IdBlockchainRepository>();
            services.AddScoped<IHotPoolRepository<IdTransaction>, HotPoolRepository>();

            return services;
        }

    }
}
