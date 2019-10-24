using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;

namespace CatchTheRun.EventProcessing
{
    public class EventProcessingConfiguration
    {
        public static IServiceCollection BuildContainer()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return ConfigureServices(configuration);
        }

        private static IServiceCollection ConfigureServices(IConfigurationRoot configurationRoot)
        {
            var services = new ServiceCollection();

            services
                .AddDefaultAWSOptions(configurationRoot.GetAWSOptions())
                .AddAWSService<IAmazonDynamoDB>(ServiceLifetime.Transient);

            return services;
        }
    }
}
