using System.IO;
using iDi.Blockchain.Framework.Execution;
using iDi.Blockchain.Framework.Extensions;
using iDi.Plus.Domain.Extensions;
using iDi.Plus.Infrastructure.Extensions;
using iDi.Plus.Node.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iDi.Plus.Node
{
    class Program
    {
        static void Main(string[] args)
        {
            // create service collection
            var services = new ServiceCollection();
            ConfigureServices(services);

            // create service provider
            var serviceProvider = services.BuildServiceProvider();

            // entry to run app
            using var scope = serviceProvider.CreateScope();
            scope.ServiceProvider.GetService<Process>()?.Run();
        }

        private static void ConfigureStages(PipelineFactory pipelineFactory)
        {
            if (pipelineFactory == null)
                return;

            //Add pipeline stage types here
            //The order is important

            //pipelineFactory.AddStage<SampleStage>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // build config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            services.AddOptions();

            services.AddDbContext<IdPlusDbContext>();

            var config = configuration.GetSection("Settings").Get<Settings>();
            services.AddSingleton(config);
            services.AddIdiBlockchainCommunicationServices()
                .AddPipeline(ConfigureStages)
                .AddIdiPlusCoreServices()
                .AddIdiInfrastructureServices(config.ConnectionString)
                .AddUpdateService()
                .AddDomainServices();

            //Add pipeline stage classes to the IoC container here
            //services.AddTransient<SampleStage>()

            // add app
            services.AddSingleton<Process>();
        }
    }
}
