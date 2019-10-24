﻿
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CatchTheRun.SubscriptionManagement
{
    public static class SubscriptionManagementConfigurationExtension
    {
        public static IServiceCollection BindAndConfigure<TConfig>(this IServiceCollection services, IConfigurationSection section, TConfig config) where TConfig : class, new()
        {
            section.Bind(config);
            services.AddSingleton(config);

            return services;
        }
    }
}