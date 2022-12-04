using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Infrastructure.Context;
using iDi.Plus.Infrastructure.Repositories;
using iDi.Plus.Infrastructure.VolatileContext;
using Microsoft.Extensions.DependencyInjection;

namespace iDi.Plus.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdiInfrastructureServices(this IServiceCollection services, string blockchainMongoDatabaseConnectionString)
        {
            services.AddScoped<IBlockchainContext>(_ => new BlockchainContext(blockchainMongoDatabaseConnectionString));
            services.AddDbContext<VolatileDbContext>();

            services.AddScoped<IIdBlockchainRepository, IdBlockchainRepository>();
            services.AddScoped<IHotPoolRepository<IdTransaction>, HotPoolRepository>();
            services.AddScoped<IConsentRepository, ConsentRepository>();

            return services;
        }

    }
}
