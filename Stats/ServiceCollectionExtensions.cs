using Microsoft.Extensions.DependencyInjection;
using Stats.Services;

namespace Stats;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddStats()
        {
            services.AddSingleton<IStatsService, StatsService>();

            return services;
        }
    }
}