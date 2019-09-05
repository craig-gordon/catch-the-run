
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TopicManagement
{
    public static class TopicManagementConfigurationExtension
    {
        public static IServiceCollection BindAndConfigure<TConfig>(this IServiceCollection services, IConfigurationSection section, TConfig config) where TConfig : class, new()
        {
            section.Bind(config);
            services.AddSingleton(config);

            return services;
        }
    }
}