using System.IO;
using iDi.Blockchain.Framework.Execution;
using iDi.Blockchain.Framework.Extensions;
using iDi.Plus.Application.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iDi.Plus.Application
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
            serviceProvider.GetService<Process>()?.Run();
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
            services.AddIdiBlockchainServer()
                .AddPipeline(ConfigureStages)
                .AddDefaultIdiPlusServices();

            //Add pipeline stage classes to the IoC container here
            //services.AddTransient<SampleStage>()

            // add app
            services.AddTransient<Process>();
        }
    }
}
