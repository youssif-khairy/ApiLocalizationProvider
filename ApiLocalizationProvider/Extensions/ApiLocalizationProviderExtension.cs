using ApiLocalizationProvider.AppSettings;
using ApiLocalizationProvider.BL;
using ApiLocalizationProvider.Controllers;
using ApiLocalizationProvider.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ApiLocalizationProvider.Extensions
{
    public static class ApiLocalizationProviderExtension
    {
        public static IServiceCollection AddApiLocalizationProvider<T>(this IServiceCollection services, Action<CacheSettings> action) where T : DbContext , ILocaliztionExtensionContext
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            services.Configure(action);

            services.AddScoped<ILocalizationProviderService, LocalizationProviderService>();

            services.AddScoped<ILocaliztionExtensionContext>(provider => provider.GetRequiredService<T>());

            var environmentControllerAssembly = typeof(LocalizationProviderController).Assembly;

            services.AddControllers()
                .AddNewtonsoftJson().PartManager.ApplicationParts
                .Add(new AssemblyPart(environmentControllerAssembly));

            return services;
        }
    }
}
