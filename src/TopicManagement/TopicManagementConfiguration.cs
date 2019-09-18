using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.SimpleNotificationService;

namespace TopicManagement
{
    public class TopicManagementConfiguration
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
                .AddAWSService<IAmazonSimpleNotificationService>(ServiceLifetime.Transient);

            return services;
        }
    }
}
