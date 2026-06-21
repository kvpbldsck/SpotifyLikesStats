using Microsoft.Extensions.DependencyInjection;
using Presenter.Services;

namespace Presenter;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddView()
        {
            services.AddSingleton<IViewService, ViewService>();

            return services;
        }
    }
}