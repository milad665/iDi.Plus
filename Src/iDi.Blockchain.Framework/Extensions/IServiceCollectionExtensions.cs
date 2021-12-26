using iDi.Blockchain.Framework.Execution;
using iDi.Blockchain.Framework.Server;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace iDi.Blockchain.Framework.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddIdiBlockchainServer(this IServiceCollection services)
        {
            services.AddScoped<IBlockchainNodeServer, DefaultBlockchainNodeServer>();

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

    }
}
