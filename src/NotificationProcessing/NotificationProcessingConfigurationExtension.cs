using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationProcessing
{
    public static class NotificationProcessingConfigurationExtension
    {
        public static IServiceCollection BindAndConfigure<TConfig>(this IServiceCollection services, IConfigurationSection section, TConfig config) where TConfig : class, new()
        {
            section.Bind(config);
            services.AddSingleton(config);

            return services;
        }
    }
}