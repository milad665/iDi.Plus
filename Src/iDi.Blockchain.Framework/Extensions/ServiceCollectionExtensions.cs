using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Execution;
using Microsoft.Extensions.DependencyInjection;
using System;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Providers;

namespace iDi.Blockchain.Framework.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdiBlockchainCommunicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IBlockchainNodeServer, DefaultBlockchainNodeServer>();
            services.AddScoped<IBlockchainNodeClient, DefaultBlockchainNodeClient>();

            return services;
        }

        public static IServiceCollection AddPipeline(this IServiceCollection services, Action<PipelineFactory> configure)
        {
            services.AddSingleton<PipelineFactory>((ctx) => {
                var pipelineFactory = new PipelineFactory(ctx);
                configure(pipelineFactory);
                return pipelineFactory;
            });
            return services;
        }

        public static IServiceCollection AddIdiPlusCoreServices(this IServiceCollection services)
        {
            services.AddScoped<CryptoServiceProvider>();
            services.AddSingleton<BlockchainNodesProvider>();
            services.AddSingleton<LocalNodeContextProvider>();

            return services;
        }
    }
}
